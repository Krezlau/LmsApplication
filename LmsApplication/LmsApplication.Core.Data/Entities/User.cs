using System.ComponentModel.DataAnnotations;

namespace LmsApplication.Core.Data.Entities;

public class User : IAuditable
{
    [Key]
    public Guid Id { get; set; }

    public Audit Audit { get; set; } = new();
    
    public string PartitionKey { get => $"{Id.ToString()}"; set { } }
}