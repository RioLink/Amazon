namespace Domain.Entities;

public class WishlistItem
{
    public int    Id        { get; set; }
    public string UserId    { get; set; } = null!;
    public User   User      { get; set; } = null!;
    public int    ProductId { get; set; }
    public Product Product  { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
