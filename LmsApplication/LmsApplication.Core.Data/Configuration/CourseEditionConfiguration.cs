using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.Core.Data.Configuration;

public class CourseEditionConfiguration : IEntityTypeConfiguration<CourseEdition>
{
    public void Configure(EntityTypeBuilder<CourseEdition> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}