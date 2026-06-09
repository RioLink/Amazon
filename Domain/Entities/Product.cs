using System;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations;
>>>>>>> Adminka
using System.Text;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
<<<<<<< HEAD
        public string Name { get; set; } = string.Empty;
=======
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range(0.01, 1000000)]
>>>>>>> Adminka
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
