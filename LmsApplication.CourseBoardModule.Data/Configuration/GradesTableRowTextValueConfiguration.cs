using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class GradesTableRowTextValueConfiguration : IEntityTypeConfiguration<GradesTableRowTextValue>
{
    public void Configure(EntityTypeBuilder<GradesTableRowTextValue> builder)
    {
        builder.Property(x => x.Value).HasColumnName("TextValue");
    }
}