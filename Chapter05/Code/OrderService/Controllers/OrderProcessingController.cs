using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace OrderService.Controllers
{
    /// <summary>
    /// this code is for illustration purpose only!
    /// </summary>
    [ApiController]  
    public class OrderController : ControllerBase
    {
        private readonly DaprClient _dapr;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger, DaprClient dapr)
        {
            _logger = logger;
            _dapr = dapr;
        }
        [Topic("daprsb", "dapr")]
        [HttpPost]
        [Route("dapr")]
        public async Task<IActionResult> ProcessOrder([FromBody] Order order)
        {
            
            _logger.LogInformation($"Order with id {order.Id} processed!");
            await PublishOrderEvent(order.Id, OrderEvent.EventType.Created);
            return Ok();
        }

        [HttpPost]
        [Route("order")]
        public async Task<IActionResult> Order([FromBody] Order order, [FromServices] DaprClient daprClient)
        {
            _logger.LogInformation($"Order with id {order.Id} created!");
            await _dapr.PublishEventAsync<Order>("daprsb", "dapr", order);            
            return Ok();
        }
        async Task<IActionResult> PublishOrderEvent(Guid OrderId, OrderEvent.EventType type)
        {
            var ev = new OrderEvent
            {
                id = OrderId,
                name = "OrderEvent",
                type = type
            };
            await _dapr.PublishEventAsync<OrderEvent>("dapreh", "shipping", ev);
            return Ok();
        }     

    }
}
