using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace changefeed
{
   /// <summary>
   /// This example is only to show how to read the changefeed of CosmosDB. For an actual event store, you should use aggregates, aggregate root etc.
   /// The below event class is by no means representative of what would really be stored in an event store.
   /// </summary>
    public class Event{       
        public string id { get; set; }        
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            CosmosClient cosmosClient = new CosmosClient("AccountEndpoint=<yourendpoint>");         
            await ReplayChangeFeed(cosmosClient);
            Console.Read();
        }
        /// <summary>
        /// Start the Change Feed Processor to listen for changes and process them with the HandleChangesAsync implementation.
        /// </summary>
        private static async Task<ChangeFeedProcessor> ReplayChangeFeed(
            CosmosClient cosmosClient)
        {
            string databaseName = "packt";
            string sourceContainerName = "events";
            string leaseContainerName = "replay";

            Container leaseContainer = cosmosClient.GetContainer(databaseName, leaseContainerName);
            ChangeFeedProcessor changeFeedProcessor = cosmosClient.GetContainer(databaseName, sourceContainerName)
                .GetChangeFeedProcessorBuilder<Event>(processorName: "replay", HandleChanges)
                    .WithInstanceName("replayservice")
                    .WithLeaseContainer(leaseContainer)
                    .WithStartTime(DateTime.MinValue.ToUniversalTime())                    
                    .Build();
            
            await changeFeedProcessor.StartAsync();            
            return changeFeedProcessor;
        }

        /// <summary>
        /// The delegate receives batches of changes as they are generated in the change feed and can process them.
        /// </summary>
        static async Task HandleChanges(
            IReadOnlyCollection<Event> changes, CancellationToken cancellationToken)
        {
   
            foreach (Event ev in changes)
            {
                Console.WriteLine($"Detected operation for item with id {ev.id}");
                await Task.Delay(10);
            }    
        }
    }
}
