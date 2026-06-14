namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? UserId { get; set; }
    public string AuthorName { get; set; } = "Анонім";
    public string Body { get; set; } = string.Empty;
    public int Stars { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
