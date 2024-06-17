using Microsoft.AspNetCore.Mvc;
using NestF.Application.DTOs.Account;
using NestF.Application.Interfaces.Services;

namespace Backend_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET
    [HttpGet("staff")]
    public async Task<IActionResult> GetStaffPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var page = await _accountService.GetStaffPageAsync(pageIndex, pageSize);
        return Ok(page);
    }

    [HttpPost("staff")]
    public async Task<IActionResult> RegisterStaffAsync([FromBody] StaffRegister model)
    {
        var staff = await _accountService.RegisterStaffAsync(model);
        return Created("/account/staff", staff);
    }

    [HttpGet("customer")]
    public async Task<IActionResult> GetCustomerPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var page = await _accountService.GetCustomerPageAsync(pageIndex, pageSize);
        return Ok(page);
    }
}