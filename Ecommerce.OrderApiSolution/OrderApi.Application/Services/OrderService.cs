﻿using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System.Data;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline): IOrderService
    {
        public async Task<ProductDTO> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/product/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;
            var product  = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/product/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;
            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);
            if(order is null || order!.Id <=0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("order-retry-pipeline");

            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));
            //var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));
            
            return new OrderDetailsDTO(
                  order.Id,
                  productDTO.Id,
                  0,//appUserDTO.Id ,
                  "",//appUserDTO.Name ,
                  "",//appUserDTO.Email ,
                  "",//appUserDTO.Address ,
                  "",//appUserDTO.TelephoneNumber,
                  productDTO.Name,
                  order.PurchaseQuantity,
                  productDTO.Price,
                  productDTO.Quantity * order.PurchaseQuantity,
                  order.OrderedDate
                );
        }

        public async  Task<IEnumerable<OrderDTO>> GetOrdersByClienId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(_ => _.ClientId == clientId);
            if (!orders.Any()) return null!;

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
