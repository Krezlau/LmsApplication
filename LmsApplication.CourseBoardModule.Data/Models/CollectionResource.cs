namespace LmsApplication.CourseBoardModule.Data.Models;

public class CollectionResource<T>
{
    public CollectionResource(IEnumerable<T> items, int totalCount)
    {
        TotalCount = totalCount;
        Items = items;
    }
    
    public int TotalCount { get; set; }
    
    public IEnumerable<T> Items { get; set; }
}