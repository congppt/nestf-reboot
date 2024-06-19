using Microsoft.AspNetCore.Mvc;
using NestF.Application.Interfaces.Services;

namespace Backend_API.Controllers;

[ApiController]
[Route("[controller]")]
[TypeFilter<ExceptionFilter>]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet("image-upload")]
    public async Task<IActionResult> GetProductPreSignedUrlAsync()
    {
        return Ok(await _productService.GetPreSignedUrlAsync());
    }
}