using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseModule.Data.Configuration;

public class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
{
    public void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}