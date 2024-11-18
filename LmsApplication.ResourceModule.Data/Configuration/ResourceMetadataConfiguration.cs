using LmsApplication.ResourceModule.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.ResourceModule.Data.Configuration;

public class ResourceMetadataConfiguration : IEntityTypeConfiguration<ResourceMetadata>
{
    public void Configure(EntityTypeBuilder<ResourceMetadata> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}