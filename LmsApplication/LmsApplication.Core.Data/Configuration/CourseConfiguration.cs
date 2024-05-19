using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.Core.Data.Configuration;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToContainer("Courses");
        builder.HasPartitionKey(x => x.PartitionKey);
        builder.HasNoDiscriminator();
        
        builder.Property(x => x.Id)
            .ToJsonProperty("id");
    }
}