using Amazon.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class ProductController : Controller
{
    public IActionResult Index(string? search)
    {
        var products = DemoCatalog.Products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products.Where(product =>
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                product.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        ViewData["Search"] = search;
        return View(products.ToList());
    }

    public IActionResult Details(int id)
    {
        var product = DemoCatalog.Products.FirstOrDefault(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}
