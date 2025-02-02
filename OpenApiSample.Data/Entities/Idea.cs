using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenApiSample.Data.Entities;

public class Idea
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? RequirementQuestions { get; set; }

    public int ProjectId { get; set; }

    public bool TechLeadApproved { get; set; }

    public bool ClientApproved { get; set; }

    public Project Project { get; set; }
}

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.ToTable("Ideas");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).HasMaxLength(500);
        builder.Property(i => i.Description).HasMaxLength(int.MaxValue);
        builder.Property(i => i.RequirementQuestions).HasMaxLength(int.MaxValue);
        builder.Property(i => i.TechLeadApproved);
        builder.Property(i => i.ClientApproved);
        builder.HasOne(i => i.Project).WithMany(p => p.Ideas).HasForeignKey(i => i.ProjectId);
    }
}
