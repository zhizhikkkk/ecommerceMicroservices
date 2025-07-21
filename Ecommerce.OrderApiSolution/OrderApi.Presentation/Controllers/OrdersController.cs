using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any()) 
                return NotFound("No order detected");
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            if (!_orders.Any()) return NotFound("No order detected");
            return Ok(_orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null) return NotFound("No order detected");
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }
        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provided");
            var orders = await orderService.GetOrdersByClienId(clientId);
            if (!orders.Any()) return NotFound("No orders of client");
            return Ok(orders);
        }
        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            try
            {
                if (orderId <= 0) return BadRequest("Invalid data provided");
                var orderDetails = await orderService.GetOrderDetails(orderId);
                return orderDetails.OrderId > 0 ? Ok(orderDetails) : NotFound("No order details here");
            }
            catch (TaskCanceledException ex)
            {
                return Ok("TimeOut exception");
            }
        }
        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDto)
        {
            if(!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");
            var orderEntity = OrderConversion.ToEntity(orderDto);
            var response = await orderInterface.CreateAsync(orderEntity);
            return  response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");
            var orderEntity = OrderConversion.ToEntity(orderDto);
            var response = await orderInterface.UpdateAsync(orderEntity);
            return response.Flag? Ok(response) : BadRequest(response);  
            
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null) return NotFound($"No order with id ={id}");
            var response = await orderInterface.DeleteAsync(order);
            return response.Flag? Ok(response) : NotFound(response);
        }
    }
}
