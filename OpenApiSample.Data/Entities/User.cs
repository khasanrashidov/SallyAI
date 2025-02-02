using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenApiSample.Data.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(256);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(256);
            builder.HasMany(u => u.Projects).WithMany(p => p.Users).UsingEntity<UserProject>();
        }
    }
}
