using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Messaging.EventGrid;

namespace HybridConnectionConsumer
{
    class Program
    {
        const string StorageBlobCreatedEvent = "Microsoft.Storage.BlobCreated";

        const string CustomTopicEvent = "Contoso.Items.ItemReceived";

        class ContosoItemReceivedEventData
        {
            public string ItemSku { get; set; }
        }

        static void Main(string[] args)
        {
            // TODO: Enter values for <relayConnectionString> and <hybridConnectionName>
            string relayConnectionString = "<relayConnectionString>";
            string hybridConnectionName = "<hybridConnectionName>";

            var hybridConnectionlistener = new HybridConnectionListener(relayConnectionString, hybridConnectionName);

            hybridConnectionlistener.RequestHandler = (context) =>
            {
                ProcessEventGridEvents(context);
                context.Response.StatusCode = System.Net.HttpStatusCode.OK;
                context.Response.Close();
            };

            hybridConnectionlistener.OpenAsync().GetAwaiter().GetResult();

            Console.WriteLine("Enter to exit the program");
            Console.ReadLine();
        }

        static void ProcessEventGridEvents(RelayedHttpListenerContext context)
        {
            var content = new StreamReader(context.Request.InputStream).ReadToEnd();
            EventGridEvent[] eventGridEvents = EventGridEvent.ParseMany(BinaryData.FromString(content));

            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                Console.WriteLine($"Received event {eventGridEvent.Id} with type:{eventGridEvent.EventType}");

                if (string.Equals(eventGridEvent.EventType, StorageBlobCreatedEvent, StringComparison.OrdinalIgnoreCase))
                {
                    // Deserialize the data portion of the event into StorageBlobCreatedEventData
                    var eventData = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
                    Console.WriteLine($"Got BlobCreated event data, blob URI {eventData.Url}");
                }
                else if (string.Equals(eventGridEvent.EventType, CustomTopicEvent, StringComparison.OrdinalIgnoreCase))
                {
                    // Deserialize the data portion of the event into ContosoItemReceivedEventData
                    var eventData = eventGridEvent.Data.ToObjectFromJson<ContosoItemReceivedEventData>();
                    Console.WriteLine($"Got ContosoItemReceived event data, item SKU {eventData.ItemSku}");
                }
                else
                {
                    // This can be extended to any event type that Event Grid supports.
                    Console.WriteLine($"Event with type {eventGridEvent.EventType} received, use proper type to deserialize data object");
                }
            }
        }
    }
}
