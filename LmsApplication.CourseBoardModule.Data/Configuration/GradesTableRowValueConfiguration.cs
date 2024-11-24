using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.CourseBoardModule.Data.Configuration;

public class GradesTableRowValueConfiguration : IEntityTypeConfiguration<GradesTableRowValue>
{
    public void Configure(EntityTypeBuilder<GradesTableRowValue> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.HasDiscriminator<string>("Type")
            .HasValue<GradesTableRowTextValue>(RowType.Text.ToString())
            .HasValue<GradesTableRowNumberValue>(RowType.Number.ToString())
            .HasValue<GradesTableRowBoolValue>(RowType.Bool.ToString());
    }
}