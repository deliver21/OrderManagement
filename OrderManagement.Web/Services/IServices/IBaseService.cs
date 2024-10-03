using OrderManagement.Web.Models;

namespace OrderManagement.Web.Services
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto);
    }
}
