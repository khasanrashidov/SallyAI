using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenApiSample.Data.Entities
{
    public class Pdw
    {
        public int Id { get; set; }

        public string JsonData { get; set; }
    }

    public class PdwConfiguration : IEntityTypeConfiguration<Pdw>
    {
        public void Configure(EntityTypeBuilder<Pdw> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.JsonData).IsRequired().HasMaxLength(int.MaxValue);
        }
    }
}
