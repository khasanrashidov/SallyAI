using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenApiSample.Data.Entities;

public class UserProject
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public User User { get; set; }
    public Project Project { get; set; }
}

public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
{
    public void Configure(EntityTypeBuilder<UserProject> builder)
    {
        builder.HasKey(up => new { up.UserId, up.ProjectId });
        builder.HasOne(u => u.User).WithMany(up => up.UserProjects).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Project).WithMany(up => up.UserProjects).HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}
