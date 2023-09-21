using System.Reflection.Metadata.Ecma335;
using API.DTOs;
using API.Entities;
using API.ExternalHelpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class UserRepository : IUserRepository
  {
    // Need to inject our DataContext (Since we query these methods from our DbContext and pass onto the Interface class)
    private readonly DataContext _context;
    private readonly IMapper _autoMapper;

    public UserRepository(DataContext context, IMapper autoMapper)
    {
      _autoMapper = autoMapper;
      _context = context;

    }

    //TODO: Caching is how we make this FASTER! --> Query from Cache rather than DB (optimize at end) 

    public async Task<MemberDto> AsyncGetMember(string username, bool currentUser) // ignore curr user so they can see their own unapproved photos
    {
      // This will retrieve users based on the provided username.
      var query = _context.Users.Where(x => x.UserName == username)

      // ProjectTo -> an  AutoMapper extension method that maps the result 
      // from the source type (AppUser) to the destination type (MemberDto).
      // essentially transforms the user data into the shape defined by MemberDto.
      .ProjectTo<MemberDto>(_autoMapper.ConfigurationProvider)

      // Convert result to an IQueryable. allows further operations 
      // to be performed on the query before it's executed against the DB
      .AsQueryable();

      // if current user making request, we ignore global query filters -> Allows currUser to see their own unapproved photos
      if (currentUser) 
      {
        query = query.IgnoreQueryFilters();
      }

      // mExecute the query against the DB and return the first result, else return null
      return await query.FirstOrDefaultAsync();
    }


    // TODO: Limit # of users being displayed (pagination)
    // AsNoTracking() => EF doesn't track changes to these objects (since we are not updating them) -> Optimization to EF

    public async Task<PaginationList<MemberDto>> AsyncGetMembers(ParameterFromUser parameterFromUser)
    {

      // We don't use this in our UserController (hence we use AsNoTracking())

      var query = _context.Users.AsQueryable();

      // build query (filter) based on paramteres from user

      // exclude current user from list we return
      query = query.Where(x => x.UserName != parameterFromUser.currUsername);

      // Gender (default is opposite) 
      query = query.Where(x => x.UserGender == parameterFromUser.gender);

      // Age filter

      // minDob (earliest Year they can be born to be under maxAge)
      var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-parameterFromUser.maxAge - 1)); // date is today (so -1)

      // maxDob (latest Year they can be born to be over minAge)
      var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-parameterFromUser.minAge));


      query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);


      // Order by filter (default is last active)

      query = parameterFromUser.orderByActive
      switch
      {
        // give newest users first (most recently created)
        "created" => query.OrderByDescending(x => x.UserCreated),
        //default - last active (has to be _ others show error?)
        _ => query.OrderByDescending(x => x.LastActive)
      };

      return await PaginationList<MemberDto>.AsyncCreate(
        // Still create our query from paginationList -> but it looks at the query we created above (so we can filter it)
        query.AsNoTracking().ProjectTo<MemberDto>(_autoMapper.ConfigurationProvider),
        parameterFromUser.PageNumber,
        parameterFromUser.PageSize);
    }

    public async Task<AppUser> AsyncGetUserByID(int id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> AsyncGetUserByUsername(string username)
    {

      return await _context.Users
      .Include(p => p.Photos)
      .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> AsyncGetUsers()
    {
      return await _context.Users
      .Include(p => p.Photos) // Eager-loading the entity (INCLUDE what else we want)
      .ToListAsync();
    }

    // get the users gender (single property -> meaning we don't have to query all field of user --> optimization for responsiveness + scalability)
    public async Task<string> GenderOfUser(string username)
    {
      return await _context.Users.Where(x => x.UserName == username).Select(s => s.UserGender).FirstOrDefaultAsync();

    }

    public async Task<AppUser> UserFromPhotoIdGetter(int id)
    {
      // start query on user table in DB
      return await _context.Users

      // want related photos for each user
      .Include(p => p.Photos)

      // remove global query filters -> can see unapproved
      .IgnoreQueryFilters()

      // want user who has photo with id we are looking for
      .Where(p => p.Photos.Any(p => p.Id == id))
      
      // get first user or null
      .FirstOrDefaultAsync();
    }



    // public async Task<bool> AsyncSaveAll() -> Read comment in IUserRepository.cs
    // {
    //   return await _context.SaveChangesAsync() > 0;
    //   // if 0 (returns false, meaning no change)
    // }

    public void Update(AppUser user)
    {
      _context.Entry(user).State = EntityState.Modified;
      // Informs EF tracker than this user has had a change
    }

  }
}