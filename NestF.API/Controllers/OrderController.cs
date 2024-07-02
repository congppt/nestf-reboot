using Microsoft.AspNetCore.Mvc;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Enums;

namespace Backend_API.Controllers;

[ApiController]
[Route("orders")]
[TypeFilter<ExceptionFilter>]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderPageAsync(int pageIndex = 0, int pageSize = 10, OrderStatus? status = null)
    {
        var page = await _orderService.GetOrderPageAsync(pageIndex, pageSize, status);
        return Ok(page);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCartPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var cart = await _orderService.GetCartPageAsync(pageIndex, pageSize);
        return Ok(cart);
    }
}