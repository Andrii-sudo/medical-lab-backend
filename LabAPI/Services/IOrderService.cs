using LabAPI.DTOs;
using LabAPI.Models;

namespace LabAPI.Services;

public interface IOrderService
{
    Task<bool> CreateOrder(CreateOrderRequest request);
    IQueryable<OrderResponse> GetOrders();
    IQueryable<OrderResponse> GetOrdersByNumber(string number);
    IQueryable<OrderResponse> GetOrdersByPatient(string patient);
    Task<(List<OrderResponse>, int)> GetPage(IQueryable<OrderResponse> ordersQuery, int page, int pageSize);
    Task<bool> PayOrder(int number);
    Task CancelOrder(int number);
}
