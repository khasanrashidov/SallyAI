using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenApiSample.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ClientName { get; set; }

        public string? Description { get; set; }

        public int? MonthsNeeded { get; set; }

        public int? State { get; set; }

        public int? Status { get; set; }

        public string? AssistantId { get; set; }

        public string? VectorStoreId { get; set; }

        public int? PdwId { get; set; }

        public Pdw? Pdw { get; set; }

        public ICollection<Member> Members { get; set; }

        public ICollection<Idea> Ideas { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<UserProject> UserProjects { get; set; }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(500);
            builder.Property(p => p.ClientName).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Description).HasMaxLength(int.MaxValue);
            builder.Property(p => p.MonthsNeeded);
            builder.Property(p => p.State);
            builder.Property(p => p.Status);
            builder.Property(p => p.AssistantId).IsRequired();
            builder.Property(p => p.VectorStoreId).IsRequired();
            builder.HasMany(p => p.Members).WithOne(m => m.Project).HasForeignKey(m => m.ProjectId);
            builder.HasMany(p => p.Ideas).WithOne(i => i.Project).HasForeignKey(i => i.ProjectId);
            builder.HasOne(p => p.Pdw).WithOne().HasForeignKey<Project>(p => p.PdwId);
        }
    }
}