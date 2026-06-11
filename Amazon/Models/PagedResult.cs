namespace Amazon.Models;

public class PagedResult<T>
{
    public List<T> Items      { get; set; } = new();
    public int Page            { get; set; }
    public int TotalPages      { get; set; }
    public int TotalItems      { get; set; }
    public bool HasPrev        => Page > 1;
    public bool HasNext        => Page < TotalPages;
}
