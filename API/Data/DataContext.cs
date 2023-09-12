using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  // Need to specify which entites we want to use as int -> already set up with int (easier to do this way)
  public class DataContext : IdentityDbContext< Roles, AppUser, int, IdentityUserClaim<int>, AppUserWithRoles,
  IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
  {
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    // public DbSet<AppUser> Users { get; set; } // table name will be users |||| IDentityCore has this property already

    public DbSet<Photo> Photos { get; set; }

    // Many to many relationship between users LikeUser (table name will be like)
    public DbSet<LikeUser> Like { get; set; }

    public DbSet<MessageUser> Message { get; set; }

    // method from DbContext class (we are overriding it)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      // Configure join between User and Roles table

      // One user can have many roles
      modelBuilder.Entity<AppUser>()

      .HasMany(u => u.AppUserRoles) 
      // AppUserRoles is the property in AppUser

      .WithOne(x => x.appUser) 
      // appUser is the property in AppUserWithRoles

      .HasForeignKey(fk => fk.UserId) 
      // foreign key in AppUserWithRoles

      .IsRequired(); // so foreign keys can't be null 


      // One role can have many users
      modelBuilder.Entity<Roles>()
      
      .HasMany(u => u.RolesUser)
      // RolesUser is the property in Roles
      
      .WithOne(x => x.appRole)
      // appRole is the property in AppUserWithRoles

      .HasForeignKey(fk => fk.RoleId)
      // foreign key in AppUserWithRoles

      .IsRequired(); // so foreign keys can't be null


      // call the base method in DbContext class and builds the model

      base.OnModelCreating(modelBuilder); 

      // configure the PRIMARY key for the LikeUser table
      modelBuilder.Entity<LikeUser>()
      .HasKey(k => new { k.SourceUserID, k.UserLikedBySourceID });

      // configure the relationship between the two entities
      modelBuilder.Entity<LikeUser>()
      .HasOne(s => s.SourceUser) // one user can like many users
      .WithMany(wm => wm.UsersLiked)
      .HasForeignKey(fk => fk.SourceUserID)
      .OnDelete(DeleteBehavior.Cascade); // if user is deleted, delete all the likes associated with that user

      // configure relation other way around
      modelBuilder.Entity<LikeUser>()
     .HasOne(s => s.UserLikedBySource) // one user can like many users
     .WithMany(wm => wm.UserLikedBy)
     .HasForeignKey(fk => fk.UserLikedBySourceID)
     .OnDelete(DeleteBehavior.Cascade); //TODO: Change to no action (can't have double .Cascade in sql for one entity, except sqlLite)


      // Message configuration

      // pk -- (if we don't use Id as the name, we have to configure it here -> EF only recognised Id as pk)

      modelBuilder.Entity<MessageUser>()
      .HasKey(k => k.messageId);



      modelBuilder.Entity<MessageUser>()
      .HasOne(x => x.ReceivingUser) // one user can receive many messages
      .WithMany(wm => wm.MessageReceived)
      .OnDelete(DeleteBehavior.Restrict); // restrict means that if user is deleted, keep the message in the DB for the other user

      modelBuilder.Entity<MessageUser>()
      .HasOne(x => x.SenderUser) // one user can send many messages
      .WithMany(wm => wm.MessageSent)
      .OnDelete(DeleteBehavior.Restrict);

    }

  }
}