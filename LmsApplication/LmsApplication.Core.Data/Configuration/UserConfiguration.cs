using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LmsApplication.Core.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToContainer("Users");
        builder.HasPartitionKey(x => x.PartitionKey);
        builder.HasNoDiscriminator();
        
        builder.Property(x => x.Id)
            .ToJsonProperty("id");
        builder.Property(d => d.ConcurrencyStamp)
            .IsETagConcurrency();
    }
}