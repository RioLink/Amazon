namespace Amazon.Models;

public class ProductCardViewModel
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required string Category { get; init; }

    public required string ImageUrl { get; init; }

    public decimal Price { get; init; }

    public decimal? OldPrice { get; init; }

    public double Rating { get; init; }

    public int ReviewCount { get; init; }

    public required string Description { get; init; }
}
