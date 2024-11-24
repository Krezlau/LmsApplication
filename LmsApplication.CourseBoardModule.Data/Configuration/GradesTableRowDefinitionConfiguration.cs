using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class GradesTableRowDefinitionConfiguration : IEntityTypeConfiguration<GradesTableRowDefinition>
{
    public void Configure(EntityTypeBuilder<GradesTableRowDefinition> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}