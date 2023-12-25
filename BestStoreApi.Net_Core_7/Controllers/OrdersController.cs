using BestStoreApi.Net_Core_7.Models;
using BestStoreApi.Net_Core_7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStoreApi.Net_Core_7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly ApplicationDbContext context;

        public OrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }



        [Authorize]
        [HttpGet]
        public IActionResult GetOrders(int? page)
        {
            int userId = JwtReader.GetUserId(User);
            string role = context.Users.Find(userId)?.Role ?? ""; //JwtReader.GetUserRole(User);

            IQueryable<Order> query = context.Orders.Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product);

            if(role != "admin")
            {
                query = query.Where(o => o.UserId == userId);

            }

            query = query.OrderByDescending(o => o.Id);

            //implement the pagination functionality

            if(page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int)(page - 1) * pageSize).Take(pageSize);



            //read the orders
            var orders = query.ToList();

            foreach(var order in orders)
            {
                //get ride of the object cycle
                foreach(var item in order.OrderItems)
                {
                    item.Order = null;

                }
                order.User.Password = "";
            }


            var response = new
            {
                Orders = orders,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page

            };

            return Ok(response);
                
        }



        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDto orderDto)
        {
            //check the payment method valid or not
            if(OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethod))
            {
                ModelState.AddModelError("Payment Method", "Please select a valid payment method");
                return BadRequest(ModelState);

            }

            int userId = JwtReader.GetUserId(User);
            var user = context.Users.Find(userId);
          
            if(user == null)
            {
                ModelState.AddModelError("Order", "Unable to create the order");
                return BadRequest(ModelState);
            }
            var productDictionary = OrderHelper.GetProductDictionary(orderDto.ProductIdentifiers);

            //create a new order
            Order order = new Order();
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;
            order.ShippingFee = OrderHelper.ShippingFee;
            order.DeliveryAddress = orderDto.PaymentMethod;
            order.PaymentStatus = OrderHelper.PaymentStatutes[0];//Pending
            order.OrderStatus = OrderHelper.OrderStatuses[0]; //Created

            foreach(var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = context.Products.Find(productId);
                if(product == null)
                {
                    ModelState.AddModelError("Product", "Product with id " + productId + " is not aviable");
                    return BadRequest(ModelState);
                }

                var orderItem = new OrderItem();
                orderItem.ProductId = productId;
                orderItem.Quantity = pair.Value;
                orderItem.UnitPrice = product.Price;

                order.OrderItems.Add(orderItem);

            }

            if(order.OrderItems.Count < 1)
            {
                ModelState.AddModelError("Order", "Unable to create the order");
                return BadRequest(ModelState);
            }

            //save the order in database
            context.Orders.Add(order);
            context.SaveChanges();

            return Ok(order);

        }
    }
}
