using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Services.IServices
{
    public interface IOrderService
    {
        Task <ResponseDto> GetAllAsync();
        Task<ResponseDto> GetAsync(int id);
        Task<ResponseDto> AddAsync(OrderDto orderDto);
        Task<ResponseDto> UpdateAsync([FromBody] OrderDto orderDto);
        Task<ResponseDto> DeleteAsync(int? id);
        Task<ResponseDto> CancelOrderAsync(int id);
        Task<ResponseDto> ProcessOrdersAsync();
    }
}
