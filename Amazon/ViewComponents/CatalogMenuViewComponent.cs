using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.ViewComponents;

public class CatalogMenuViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CatalogMenuViewComponent(ICategoryService categoryService)
        => _categoryService = categoryService;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }
}
