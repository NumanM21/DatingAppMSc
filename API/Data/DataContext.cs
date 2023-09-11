using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<AppUser> Users { get; set; } // table name will be users

    public DbSet<Photo> Photos { get; set; }

    // Many to many relationship between users LikeUser (table name will be like)
    public DbSet<LikeUser> Like { get; set; }

    public DbSet<MessageUser> Message { get; set; }

    // method from DbContext class (we are overriding it)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // call the base method in DbContext class and builds the model

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