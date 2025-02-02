using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenApiSample.Data.Entities;

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.FullName).IsRequired().HasMaxLength(500);
        builder.HasOne(m => m.Project).WithMany(p => p.Members).HasForeignKey(m => m.ProjectId);
    }
}
