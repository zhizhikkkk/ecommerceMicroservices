
using OrderApi.Application.DTOs;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task <IEnumerable<OrderDTO>> GetOrdersByClienId(int  clienId);
        Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    }
}
