using Amazon.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }   

    public async Task<IActionResult> Index(string? search)
    {
        var products = (await _productService.GetAllProductsAsync()).Select(p => new ProductCardViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category.Name,
            Price = p.Price,
            Description = p.Description,
            ImageUrl = p.ImageUrl
        }).ToList();


        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products.Where(product =>
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                product.Category.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        ViewData["Search"] = search;
        return View(products.ToList());
    }

    public async Task<IActionResult> Details(int id)
    { 
        var product = await _productService.GetProductByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return View(new ProductCardViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl
        });
    }
}
