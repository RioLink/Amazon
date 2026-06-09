using BLL.Interfaces;
using DAL.Repositories.Interfaces;
using Domain.Entities;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Category> _categoryRepo;

        public ProductService(IGenericRepository<Product> productRepo, IGenericRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> AddProductAsync(string name, decimal price, int categoryId, string description, string imageUrl, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Назва не може бути порожньою");

            if (price <= 0)
                return (false, "Ціна має бути більше 0");

            if (quantity < 0)
                return (false, "Кількість не може бути від'ємною");

            var category = await _categoryRepo.GetByIdAsync(categoryId);

            if (category == null)
                return (false, "Оберіть категорію");
            
            if (string.IsNullOrWhiteSpace(description))
                return (false, "Опис не може бути порожнім");


            if (string.IsNullOrWhiteSpace(imageUrl))
                return (false, "Посилання на зображення не може бути порожнім");

            if (quantity < 0)
                return (false, "Кількість не може бути від'ємною");

            var newProduct = new Product 
            {
                Name = name.Trim(),
                Price = price,
                Category = category,
                Description = description.Trim(),
                ImageUrl = imageUrl.Trim(),
                Quantity = quantity
            };

            await _productRepo.AddAsync(newProduct);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(int id, string name, decimal price, int categoryId, string description, string imageUrl, int quantity)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
                return (false, "Продукт не знайдено.");
                
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Назва не може бути порожньою");

            if (price <= 0)
                return (false, "Ціна має бути більше 0");

            if (quantity < 0)
                return (false, "Кількість не може бути від'ємною");

            var category = await _categoryRepo.GetByIdAsync(categoryId);

            if (category == null)
                return (false, "Оберіть категорію");

            if (string.IsNullOrWhiteSpace(description))
                return (false, "Опис не може бути порожнім");


            if (string.IsNullOrWhiteSpace(imageUrl))
                return (false, "Посилання на зображення не може бути порожнім");

            if (quantity < 0)
                return (false, "Кількість не може бути від'ємною");

            product.Name = name.Trim();
            product.Price = price;
            product.Category = category;
            product.CategoryId = categoryId;
            product.Description = description.Trim();
            product.ImageUrl = imageUrl.Trim();
            product.Quantity = quantity;

            await _productRepo.UpdateAsync(product);
            return (true, string.Empty);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepo.DeleteAsync(id);
        }
    }
}