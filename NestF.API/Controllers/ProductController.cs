using Microsoft.AspNetCore.Mvc;
using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Product;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;

namespace Backend_API.Controllers;

[ApiController]
[Route("products")]
[TypeFilter<ExceptionFilter>]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var page = await _productService.GetProductPageAsync(pageIndex, pageSize);
        return Ok(page);
    }
    [HttpPost("{id}/image-upload")]
    public async Task<IActionResult> GetProductPreSignedUrlAsync(int id)
    {
        return Ok(await _productService.GetPreSignedUrlAsync(id));
    }

    [HttpGet("new")]
    public async Task<IActionResult> CreateProductAsync()
    {
        var product = await _productService.CreateProductAsync();
        return Created($"product/{product.Id}", product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] ProductUpdate model)
    {
        var product = await _productService.UpdateProductAsync(id, model);
        return Ok(product);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> AddProductImageAsync(int id, [FromBody] ImageWebhook model)
    {
        await _productService.AddProductImageAsync(id, model.ImagePath);
        return Ok();
    }
}