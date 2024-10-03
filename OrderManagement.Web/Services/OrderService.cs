using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Models;
using OrderManagement.Web.Services.IServices;
using OrderManagement.Web.Utilities;

namespace OrderManagement.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService) { 
            _baseService = baseService;
        }
        public Task<ResponseDto> AddAsync(OrderDto orderDto)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType=SD.ApiType.POST,
                Data=orderDto,
                Url=SD.OrderAPIBase+"/api/order"
            });
        }

        public Task<ResponseDto> CancelOrderAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = id,
                Url = SD.OrderAPIBase + "/api/order/CancelOrder"
            });
        }

        public Task<ResponseDto> DeleteAsync(int? id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.OrderAPIBase + "/api/order/" +id
            });
        }

        public Task<ResponseDto> GetAllAsync()
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase+"/api/order"
            });
        }

        public Task<ResponseDto> GetAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/" +id
            });
        }

        public Task<ResponseDto> ProcessOrdersAsync()
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.OrderAPIBase + "/api/order/processOrders"
            });
        }

        public Task<ResponseDto> UpdateAsync([FromBody] OrderDto orderDto)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data=orderDto,
                Url = SD.OrderAPIBase + "/api/order"
            });
        }
    }
}
