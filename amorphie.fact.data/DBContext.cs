
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace amorphie.fact.data;


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
    public DbSet<UserDevice>? UserDevices { get; set; }
    public DbSet<SecurityQuestion>? SecurityQuestions { get; set; }
    public DbSet<SecurityImage>? SecurityImages { get; set; }
    public DbSet<UserSecurityImage>? UserSecurityImages { get; set; }
    public DbSet<UserPassword>? UserPasswords { get; set; }
    public DbSet<DeactiveDefinition>? DeactiveDefinitions { get; set; }
    public DbSet<Client>? Clients { get; set; }
    public DbSet<ClientToken>? ClientTokens { get; set; }
    public DbSet<ClientGrantType>? ClientGrantTypes { get; set; }
    public DbSet<ClientFlow>? ClientFlows { get; set; }
    public DbSet<ClientAudience>? ClientAudiences { get; set; }
    public DbSet<UserSmsKey>? UserSmsKeys { get; set; }

    public UserDBContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>()
       .HasMany(b => b.UserTags);

        modelBuilder.Entity<User>()
        .HasIndex(e => e.Reference).IsUnique();

        modelBuilder.Entity<UserSecurityQuestion>()
     .HasIndex(e => e.SecurityQuestionId).IsUnique();

        modelBuilder.Entity<UserSecurityQuestion>()
     .HasIndex(e => e.UserId).IsUnique();

        //      modelBuilder.Entity<UserSecurityImage>()
        // .HasIndex(e => e.UserId).IsUnique();

        modelBuilder.Entity<User>().OwnsOne(e => e.Phone);



        modelBuilder.Entity<UserTag>()
       .HasIndex(b => new { b.Id, b.Tag, b.UserId })
        .HasMethod("GIN")
       .IsTsVectorExpressionIndex("english");



        modelBuilder.Entity<UserDevice>()
       .HasIndex(b => new { b.Id, b.DeviceId, b.UserId, b.ClientId })
        .HasMethod("GIN")
       .IsTsVectorExpressionIndex("english");


        modelBuilder.Entity<SecurityQuestion>()
     .HasIndex(b => new { b.Id, b.Question })
      .HasMethod("GIN")
     .IsTsVectorExpressionIndex("english");

        modelBuilder.Entity<UserSecurityQuestion>()
.HasIndex(b => new { b.Id, b.UserId, b.SecurityQuestionId })
.HasMethod("GIN")
.IsTsVectorExpressionIndex("english");

        modelBuilder.Entity<UserSmsKey>()
        .HasIndex(b => new { b.Id, b.UserId, b.SmsKey })
        .HasMethod("GIN")
        .IsTsVectorExpressionIndex("english");


        modelBuilder.Entity<User>().HasIndex(item => item.SearchVector).HasMethod("GIN");
        modelBuilder.Entity<User>().Property(item => item.SearchVector).HasComputedColumnSql(FullTextSearchHelper.GetTsVectorComputedColumnSql("english", new string[] { "Reference", "EMail", "FirstName", "LastName", "Number" }), true); //We have to manually specify the generated SQL since we are using columns spanning the split table.

        modelBuilder.Entity<Client>().HasIndex(item => item.SearchVector).HasMethod("GIN");
        modelBuilder.Entity<Client>().Property(item => item.SearchVector).HasComputedColumnSql(FullTextSearchHelper.GetTsVectorComputedColumnSql("english", new string[] { "ReturnUrl", "LoginUrl", "LogoutUrl" }), true);
       
    }
}




