using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class GradesTableRowNumberValueConfiguration : IEntityTypeConfiguration<GradesTableRowNumberValue>
{
    public void Configure(EntityTypeBuilder<GradesTableRowNumberValue> builder)
    {
        builder.Property(x => x.Value)
            .HasColumnName("NumberValue")
            .HasColumnType("decimal(18,2)");
    }
}