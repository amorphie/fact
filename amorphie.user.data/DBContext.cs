
using amorphie.user.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace amorphie.user.data;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDBContext>
{
    public UserDBContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UserDBContext>();

        var connStr = "Host=localhost:5432;Database=users;Username=postgres;Password=postgres";
        builder.UseNpgsql(connStr);
        return new UserDBContext(builder.Options);
    }
}

public class UserDBContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    public DbSet<UserSecurityQuestion>? UserSecurityQuestions { get; set; }

    public DbSet<UserTag>? UserTags { get; set; }
    

    public UserDBContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(e => e.Id);

      
        modelBuilder.Entity<UserSecurityQuestion>()
            .HasKey(e => e.Id);
            

        modelBuilder.Entity<UserTag>()
            .HasKey(e => e.Id);


        var UserId = Guid.NewGuid();

        modelBuilder.Entity<User>().HasData(new
        {
            Id = UserId,
             Name= "Damla",
           Surname="Erhan",
           Password="123",
           TcNo="12345678912",
         
        });
 
        modelBuilder.Entity<UserSecurityQuestion>().HasData(new
        {
            Id =  Guid.NewGuid(),
           SecurityQuestion="en sevdiğiniz araba modeli",
           UserId=UserId,
         
        });
      
            modelBuilder.Entity<UserTag>().HasData(new
        {
            Id = Guid.NewGuid(),
           Name="user-list-get",
           UserId=UserId,
         
        });
        
    }
}





