using static OrderManagement.Web.Utilities.SD;

namespace OrderManagement.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }    
    }
}
