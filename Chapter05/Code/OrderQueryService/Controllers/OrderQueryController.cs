using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace OrderQueryService.Controllers
{
    /// <summary>
    /// this code is for illustration purpose only!
    /// </summary>
    [ApiController]   
    public class OrderQueryController : ControllerBase
    {
        private static readonly Order[] orders = new[]
        {
           new Order
           {
               Id=new Guid("4aadc0f8-eeda-4ee7-9c26-a6d39cbfbc28"),
               Products = new List<Product>{
                   new Product{Id=new Guid("5678f982-2ae4-408c-92ff-6af45118d159"),Name="fake product 1"},
                   new Product{Id=new Guid("c645fbaf-80d3-471b-bb5c-04352c0e4ed1"),Name="fake product 2"},
               }
           },
           new Order
           {
               Id=new Guid("f31ec6f3-5076-4d7c-8c35-53edbd2711c2"),
               Products = new List<Product>{
                   new Product{Id=new Guid("5678f982-2ae4-408c-92ff-6af45118d159"),Name="fake product 1"}                   
               }
           }
        };

        private readonly ILogger<OrderQueryController> _logger;

        public OrderQueryController(ILogger<OrderQueryController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            string s = id.ToString();
            _logger.LogInformation(s);
            _logger.LogInformation("checking for order {id}");
            var order = orders.Where(o => o.Id.Equals(id));
            if (order.Count() == 0)
            {
                _logger.LogInformation("order not found");
                return NotFound();
            }
                
            return Ok(order.FirstOrDefault());
        }
    }
}
