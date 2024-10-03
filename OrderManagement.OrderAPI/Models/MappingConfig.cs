using AutoMapper;
using OrderManagement.OrderAPI.Models.DTO;

namespace OrderManagement.OrderAPI.Models
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderDto,Order>().ReverseMap();
            });
            return mappingconfig;
        }
    }
}
