using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulatorConsole
{
    class Program
    {
        static int instances = 2;
        static int count = 0;
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            if (args.Length == 1)
            {
                instances = Convert.ToInt32(args[0]);
            }

            var parallelTasks = new List<Task>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await using (var producerClient = new EventHubProducerClient(config["EventHubCs"], "data"))
            {
                
                for (int i = 0; i < instances; i++)
                {
                    parallelTasks.Add(Task.Run(async () =>
                    {
                        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                        for (int i = 0;i<25;i++)
                        {
                            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(
                            JsonSerializer.Serialize(new DataObject())
                            )));
                            
                        }
                        Interlocked.Add(ref count, eventBatch.Count);
                        await producerClient.SendAsync(eventBatch);
                        

                    }));
                   
                }
                await Task.WhenAll(parallelTasks);
            }
            sw.Stop();
            Console.WriteLine("Send {0} events in {1} seconds", count,(sw.ElapsedMilliseconds/1000));


         }
       
    }
    public class DataObject
    {
        private string[] sensorNames = new string[] { "Brussels", "Genval" };
        public string sensorName { get; private set; }
        public double speed { get; private set; }
        public string plateNumber { get; private set; }
        public DataObject()
        {
            sensorName = sensorNames[new Random().Next(0, 2)];            
            speed = (new Random().NextDouble()*100);
            plateNumber = Guid.NewGuid().ToString();
        }
    }
}
