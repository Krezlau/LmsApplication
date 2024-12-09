using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class FinalGradeConfiguration : IEntityTypeConfiguration<FinalGrade>
{
    public void Configure(EntityTypeBuilder<FinalGrade> builder)
    {
        builder.HasIndex(x => new { x.CourseEditionId, x.UserId }).IsUnique();
    }
}