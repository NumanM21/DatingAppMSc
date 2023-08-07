using System.Reflection.Metadata.Ecma335;
using API.DTOs;
using API.Entities;
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

    //TODO: Caching is how we make this FASTER! --> Query from Cache rather than DB (optimize at end) TODO:
    public async Task<MemberDto> AsyncGetMember(string username)
    {
      return await _context.Users
      .Where(x => x.UserName == username)
      .ProjectTo<MemberDto>(_autoMapper.ConfigurationProvider)
      .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> AsyncGetMembers()
    {
      return await _context.Users
      .ProjectTo<MemberDto>(_autoMapper.ConfigurationProvider)
      .ToListAsync();
    }

    public async Task<AppUser> AsyncGetUserByID(int id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> AsyncGetUserByUsername(string username)
    {
      // TODO: Not efficient => Querying unwanted data from AppUser -> Better to get the Dto itself (contains only fields we require)
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

    public async Task<bool> AsyncSaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
      // if 0 (returns false, meaning no change)
    }

    public void Update(AppUser user)
    {
      _context.Entry(user).State = EntityState.Modified;
      // Informs EF tracker than this user has had a change
    }

  }
}