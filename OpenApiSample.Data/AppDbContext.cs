using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenApiSample.Data.Entities;
using System.Reflection;

namespace OpenApiSample.Data;

public class AppDbContext : IdentityDbContext<User, Role, int>
{
    public AppDbContext() { }

    public AppDbContext(
        DbContextOptions options
    )
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Idea> Ideas => Set<Idea>();

    public DbSet<UserProject> UserProjects => Set<UserProject>();

    public DbSet<Pdw> Pdws => Set<Pdw>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<User>()
            .Ignore(u => u.AccessFailedCount)
            .Ignore(u => u.LockoutEnabled)
            .Ignore(u => u.LockoutEnd)
            .Ignore(u => u.TwoFactorEnabled)
            .Ignore(u => u.PhoneNumberConfirmed);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }


    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}