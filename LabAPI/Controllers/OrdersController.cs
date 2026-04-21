using LabAPI.Constants;
using LabAPI.DTOs;
using LabAPI.Models;
using LabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpPost()]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {   
        if (!await _orderService.CreateOrder(request))
        {
            return BadRequest(new { msg = "Не вдалося стоворити замовлення" });
        }

        return Created();
    }

    [Authorize(Roles=($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-number")]
    public async Task<IActionResult> GetOrdersByNumber(int page, int pageSize, string? number)
    {
        var query = _orderService.GetOrders();
        if (!string.IsNullOrWhiteSpace(number))
        {
            query = _orderService.GetOrdersByNumber(number);
        }

        var (orders, pageCount) = await _orderService.GetPage(query, page, pageSize);
        
        return Ok(new
        {
            Orders = orders,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpGet("by-patient")]
    public async Task<IActionResult> GetOrdersByPatient(int page, int pageSize, string? patient)
    {
        var query = _orderService.GetOrders();
        if (!string.IsNullOrWhiteSpace(patient))
        {
            query = _orderService.GetOrdersByPatient(patient);
        }

        var (orders, pageCount) = await _orderService.GetPage(query, page, pageSize);
        
        return Ok(new
        {
            Orders = orders,
            PageCount = pageCount
        });
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpPut("{number}/pay")]
    public async Task<IActionResult> PayOrder(int number)
    {
        if (!await _orderService.PayOrder(number))
        {
            return BadRequest();
        }

        return Ok();
    }

    [Authorize(Roles = ($"{Roles.Admin},{Roles.Employee}"))]
    [HttpPut("{number}/cancel")]
    public async Task<IActionResult> CancelOrder(int number)
    {
        await _orderService.CancelOrder(number);

        return Ok();
    }
}
