using Microsoft.AspNetCore.Mvc;
using NestF.Application.DTOs.Account;
using NestF.Application.Interfaces.Services;

namespace Backend_API.Controllers;

[ApiController]
[Route("accounts")]
[TypeFilter<ExceptionFilter>]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET
    [HttpGet("staffs")]
    public async Task<IActionResult> GetStaffPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var page = await _accountService.GetStaffPageAsync(pageIndex, pageSize);
        return Ok(page);
    }

    [HttpPost("staff")]
    public async Task<IActionResult> RegisterStaffAsync([FromBody] StaffRegister model)
    {
        var staff = await _accountService.RegisterStaffAsync(model);
        return Created($"/account/staff/{staff.Id}", staff);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomerPageAsync(int pageIndex = 0, int pageSize = 10)
    {
        var page = await _accountService.GetCustomerPageAsync(pageIndex, pageSize);
        return Ok(page);
    }

    [HttpGet("customers/{id}")]
    public async Task<IActionResult> GetCustomerDetailAsync(int id)
    {
        var customer = await _accountService.GetCustomerDetailAsync(id);
        return Ok(customer);
    }

    [HttpGet("staffs/{id}")]
    public async Task<IActionResult> GetStaffDetailAsync(int id)
    {
        var staff = await _accountService.GetStaffDetailAsync(id);
        return Ok(staff);
    }

    [HttpPost("customer")]
    public async Task<IActionResult> RegisterCustomerAsync([FromBody] CustomerRegister model)
    {
        var customer = await _accountService.RegisterCustomerAsync(model);
        return Created($"account/customer/{customer.Id}", customer);
    }
}