using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO dto) => new()
        {
            Id= dto.Id,
            ProductId = dto.ProductId,
            ClientId = dto.ClientId,
            PurchaseQuantity = dto.ProductQuantity,
            OrderedDate = dto.OrderedDate
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order order, IEnumerable<Order>? orders) 
        {
            if(order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderedDate
                    );
                return new(singleOrder, null);
            }

            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o=>new OrderDTO(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderedDate)).ToList();
                return new(null, _orders);
            }
            return new (null, null);
        }
    }
}
