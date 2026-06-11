using Microsoft.AspNetCore.Http;

namespace Amazon.Models
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public int Quantity { get; set; }
    }
}
