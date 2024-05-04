namespace LmsApplication.Core.Data.Entities;

public interface IAuditable
{
    public Audit Audit { get; set; }
    
    public string PartitionKey { get; set; }
}

public class Audit
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}