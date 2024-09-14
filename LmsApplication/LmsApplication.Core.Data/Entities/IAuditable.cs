namespace LmsApplication.Core.Data.Entities;

public interface IAuditable
{
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
}