using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.Models;
using Newtonsoft.Json;
using OrderManagement.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Azure.Core.HttpHeader;

namespace OrderManagement.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) 
        {
            _orderService = orderService;
        }    
        public async Task<IActionResult> Index()
        {
            ResponseDto responseDto = new();
            responseDto = await _orderService.GetAllAsync();
            List<OrderDto> objList=new List<OrderDto>();
            if(responseDto != null && responseDto.IsSuccess) {
                objList = JsonConvert.DeserializeObject<List<OrderDto>>(Convert.ToString(responseDto.Result));           
            }
            return View(objList);
        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> currencies= new List<SelectListItem>() { 
                new SelectListItem{Value="USD",Text="USD"},
                new SelectListItem{Value="EUR",Text="EUR"}
            };
            ViewBag.CurrencyList= currencies;
            return View();
        }
        [HttpPost]
		public async Task<IActionResult> Create(OrderDto order)
		{
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseDto? response = await _orderService.AddAsync(order);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Order is successfully created";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch(Exception ex) {
                TempData["error"] = "Create Order failed \n"+ex.Message.ToString();

            }
            return View(order);
		}
        public async Task<IActionResult>Update(int orderId)
        {
            try
            {
                ResponseDto response = new();
                response = await _orderService.GetAsync(orderId);
                if(response!=null && response.IsSuccess)
                {
                    OrderDto orderDto = new();
                    orderDto = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(response.Result));
                    return View(orderDto);
                }
            }
            catch(Exception ex)
            {
                TempData["error"] = "Something went wrong!\n"+ex.Message.ToString();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(OrderDto orderDto)
        {
            try
            {
                ResponseDto response = new();
                response = await _orderService.UpdateAsync(orderDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Order is successfully updated ";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong!\n"+ex.Message.ToString();
            }
            return View(orderDto);
        }
        public async Task<IActionResult> Cancel(int orderId)
        {
			OrderDto orderDto = new();
			try
            {
                ResponseDto response = new();
                response = await _orderService.CancelOrderAsync(orderId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Order Status is successfully updated to Cancel ";
                }
                else
                {
                    TempData["error"] = "Something went wrong!\n"+response.Message.ToString();                    
                }
				response = await _orderService.GetAsync(orderId);				
				orderDto = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(response.Result));
			}
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong!\n" + ex.Message.ToString();
            }
			return View("Update", orderDto);
		}
        public async Task<IActionResult> Delete(int orderId)
        {            
            OrderDto orderDto = new();
            try
            {
                ResponseDto responseDto = new();
                responseDto =await _orderService.GetAsync(orderId);
                if(responseDto!=null && responseDto.IsSuccess)
                {
                    orderDto = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(responseDto.Result));
                    return View(orderDto);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong!\n" + ex.Message.ToString();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(OrderDto orderDto)
        {
            try
            {
                ResponseDto responseDto= new();
                responseDto = await _orderService.DeleteAsync(orderDto.Id);
                if(responseDto!= null && responseDto.IsSuccess)
                {
                    TempData["success"] = "Order is successfully deleted";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["error"] = "Something went wrong!\n" + ex.Message.ToString();
            }
            return View(orderDto.Id);
        }
		public async Task<IActionResult> ProcessOrders()
        {
            try
            {
                ResponseDto response = await _orderService.ProcessOrdersAsync();
                if(response.IsSuccess)
                {
                    TempData["success"] = "Orders are successfully performed";
                }
                else
                {
                    TempData["error"] = "Something went wrong!";

				}
            }
            catch(Exception ex)
            {
				TempData["error"] = "Something went wrong!\n" + ex.Message.ToString();
			}
            return RedirectToAction("Index");   
        }

	}
}
