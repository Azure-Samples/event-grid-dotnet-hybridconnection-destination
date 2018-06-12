using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Relay;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HybridConnectionConsumer
{
    class Program
    {
        const string StorageBlobCreatedEvent = "Microsoft.Storage.BlobCreated";

        const string CustomTopicEvent = "Contoso.Items.ItemReceivedEvent";

        class ContosoItemReceivedEventData
        {
            [JsonProperty(PropertyName = "itemSku")]
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
            EventGridEvent[] eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(content);

            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                Console.WriteLine($"Received event {eventGridEvent.Id} with type:{eventGridEvent.EventType}");
                JObject dataObject = eventGridEvent.Data as JObject;

                if (string.Equals(eventGridEvent.EventType, StorageBlobCreatedEvent, StringComparison.OrdinalIgnoreCase))
                {
                    // Deserialize the data portion of the event into StorageBlobCreatedEventData
                    var eventData = dataObject.ToObject<StorageBlobCreatedEventData>();
                    Console.WriteLine($"Got BlobCreated event data, blob URI {eventData.Url}");
                }
                else if (string.Equals(eventGridEvent.EventType, CustomTopicEvent, StringComparison.OrdinalIgnoreCase))
                {
                    // Deserialize the data portion of the event into ContosoItemReceivedEventData
                    var eventData = dataObject.ToObject<ContosoItemReceivedEventData>();
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
