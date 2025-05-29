using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Mango.Web.Services;
using Newtonsoft.Json;
using Mango.Web.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    //private readonly ICartService _cartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto>? list = new();

        ResponseDto? response = await _productService.GetAllProductsAsync();

        if (response != null && response.Success)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(list);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ProductDto? model = new();

        ResponseDto? response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.Success)
        {
            model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(model);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
