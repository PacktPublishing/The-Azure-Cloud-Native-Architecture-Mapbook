using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Dapr.Client;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Dapr.Client.Http;
using Newtonsoft.Json;

namespace TrackingService.Controllers
{
    /// <summary>
    /// this code is for illustration purpose only!
    /// </summary>
    [ApiController]   
    public class ShippingController : ControllerBase
    {
        private readonly DaprClient _dapr;

        private readonly ILogger<ShippingController> _logger;

        public ShippingController(ILogger<ShippingController> logger, DaprClient dapr)
        {
            _logger = logger;
            _dapr = dapr;
        }      

        [Topic("dapreh", "shipping")]
        [HttpPost]
        [Route("dapr")]      
        public async Task<IActionResult> ProcessOrderEvent([FromBody] OrderEvent ev)
        {

            _logger.LogInformation($"Received new event");
            _logger.LogInformation("{0} {1} {2}", ev.id, ev.name, ev.type);

            switch (ev.type)
            {
                case OrderEvent.EventType.Created:
                    if (await GetOrder(ev.id))
                    {
                        _logger.LogInformation($"Starting shipping process for order {ev.id}!");
                    }
                    else
                    {
                        _logger.LogInformation($"order {ev.id} not found!");
                    }
                    break;
                case OrderEvent.EventType.Updated:
                    if (await GetOrder(ev.id))
                    {
                        _logger.LogInformation($"Checking shipping process impact for order {ev.id}!");
                    }
                    else
                    {
                        _logger.LogInformation($"order {ev.id} not found, cancelling shipping process if any!");
                    }
                    break;
                case OrderEvent.EventType.Deleted:
                    _logger.LogInformation($"Cancelling shipping process for order {ev.id}!");
                    break;
            }

            return Ok();
        }
      
        async Task<bool> GetOrder(Guid id)
        {
            HTTPExtension ext = new HTTPExtension();
            ext.Verb = HTTPVerb.Get;
            try
            {
                await _dapr.InvokeMethodAsync<object, Order>(
               "orderquery",
               id.ToString(), null, ext);
                return true;
            }            
            catch (Exception ex)
            {
                if(((Grpc.Core.RpcException)ex.InnerException).StatusCode == Grpc.Core.StatusCode.NotFound)
                    return false;
                //else ==> should handle the other cases or rely on retry policies of a service mesh
            }
            return false;            
        }

    }
}
