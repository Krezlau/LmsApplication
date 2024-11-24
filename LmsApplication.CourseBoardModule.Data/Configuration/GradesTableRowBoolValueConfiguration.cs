using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class GradesTableRowBoolValueConfiguration : IEntityTypeConfiguration<GradesTableRowBoolValue>
{
    public void Configure(EntityTypeBuilder<GradesTableRowBoolValue> builder)
    {
        builder.Property(x => x.Value)
            .HasColumnName("BoolValue");
    }
}