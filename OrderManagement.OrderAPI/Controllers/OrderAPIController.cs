using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.OrderAPI.Data;
using OrderManagement.OrderAPI.Models;
using OrderManagement.OrderAPI.Models.DTO;
using OrderManagement.OrderAPI.Services;
using OrderManagement.OrderAPI.Services.IServices;
using OrderManagement.OrderAPI.Utilities;
using static Azure.Core.HttpHeader;

namespace OrderManagement.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderDto> _validator;   
        private readonly IOrderProcessingService _orderProcessingService; 
        private readonly ICurrencyService _currencyService;
        public OrderAPIController(ApplicationDbContext db , IMapper mapper,IValidator<OrderDto> validator
            ,IOrderProcessingService orderProcessingService,ICurrencyService currencyService) { 
            _db = db;
            _mapper = mapper;
            _validator = validator;
            _orderProcessingService=orderProcessingService;
            _responseDto= new ResponseDto();
            _currencyService = currencyService;
        }
        [HttpGet]
        public ResponseDto Get()
        {            
            try
            {
                IEnumerable<Order> objList = _db.Orders.AsNoTracking().ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<OrderDto>>(objList);
            }
            catch(Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var orderDto = await _db.Orders.AsNoTracking().FirstAsync(u=>u.Id==id);
                _responseDto.Result = _mapper.Map<OrderDto>(orderDto);
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
        [HttpPost]
        public async Task<ResponseDto> Post([FromBody]OrderDto orderDto)
        {
            try
            {
                // Manually validate the OrderDto using FluentValidation
                var validationResult = await _validator.ValidateAsync(orderDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(err => new { err.PropertyName, err.ErrorMessage });
                    _responseDto.Result = errors;
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Bad request";
                }    
                else
                {
                    var order = _mapper.Map<Order>(orderDto);
                    order.Status = SD.StatusPending;
                    order.OrderDate = DateTime.Now;
                    var getCurrency = await _currencyService.GetExchangeRate(order.Currency);
                    if (getCurrency!=null)
                    {
                        order.Priority = (int)(order.TotalAmount * (1 / getCurrency));
                    }
                    else { order.Priority = 0; }//todo Ask for the currency dependecy
                    await _db.Orders.AddAsync(order);
                    await _db.SaveChangesAsync();
                    _responseDto.Result=_mapper.Map<OrderDto>(order);
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] OrderDto orderDto)
        {
            try
            {
                // Manually validate the OrderDto using FluentValidation
                var validationResult = await _validator.ValidateAsync(orderDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(err => new { err.PropertyName, err.ErrorMessage });
                    _responseDto.Result = errors;
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Bad request";
                }
                else
                {
                    var order = _mapper.Map<Order>(orderDto);
                    _db.Orders.Update(order);
                    await _db.SaveChangesAsync();
                    _responseDto.Result = _mapper.Map<OrderDto>(order);
                    
                }
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ResponseDto> Delete(int? id)
        {
            try
            {
                var order = _db.Orders.First(u => u.Id == id);
                _db.Remove(order);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _responseDto.Message=ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
        [HttpPut("processOrders")]
        public async Task<ResponseDto> ProcessOrders()
        {
            try
            {
                await _orderProcessingService.ProcessPendingOrdersAsync();                
                _orderProcessingService.ProcessCompletedOrdersAsync();
                IEnumerable<Order> list = _db.Orders;
                _responseDto.Result = _mapper.Map<IEnumerable<OrderDto>>(list);
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Order processing started.";
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }
        [HttpPut("CancelOrder")] 
        public async Task<ResponseDto> CancelOrder([FromBody]int id)
        {
            try
            {
                Order order = new();
                order=await _db.Orders.FirstAsync(u => u.Id == id);                
                if(order!=null)
                {
                    if(order.Status!=SD.StatusCompleted)
                    {
                        order.Status = SD.StatusCancelled;
                        order.Priority = 0;//todo ask for
                        _db.Orders.Update(order);                        
                        await _db.SaveChangesAsync();                        
                        _responseDto.Result = _mapper.Map<OrderDto>(order);
                    }
                    else
                    {
                        _responseDto.Result = order.Status;
                        _responseDto.IsSuccess=false;
                        _responseDto.Message = "The current order is Completed";
                    }
                }
            }
            catch(Exception ex )
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message= ex.Message.ToString();
            }
            return _responseDto;
        }

    }
}
