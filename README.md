---
services: event-grid
platforms: dotnet
author: kishp
---

# Microsoft Azure Event Grid Hybrid Connection Consumer sample for C#

This contains a C# sample to consume events from Azure Event Grid using an Azure relay hybrid connection listener.

## Features

This sample demonstrates how to process event grid events which are delivered to an Azure relay hybrid connection destination.

The above sample uses the Event Grid data plane SDK (Microsoft.Azure.EventGrid) and Azure Relay SDK (Microsoft.Azure.Relay).

## Getting Started

### Prerequisites

- .NET Core 2.0 or higher
 1. Create an Azure Event Grid topic: You will need to first create an Event Grid topic. The steps are described at https://docs.microsoft.com/en-us/azure/event-grid/scripts/event-grid-cli-create-custom-topic. Make a note of the topic details. 
 2. Create an Azure Relay Hybrid connection. https://docs.microsoft.com/en-us/azure/service-bus-relay/relay-hybrid-connections-http-requests-dotnet-get-started. Make a note of the hybrid connection details. 
 3. Create an event subscription on the topic(created in step#1) using hybrid connection as a destination (created in step#2) using following azure CLI commands 
    ```azurecli-interactive

    # Replace with relay hybrid connection details

    relayazuresubscriptionid=<relay-azure-subscription-id>
    relayrg=<relay-resource-group>
    relayname=<relay-namespace-name>
    hybridname=<hybrid-connection-name>

    # Replace with topic details
    topicazuresubscriptionid=<topic-azure-subscription-id>
    topicresourcegroup=<topic-resource-group>
    topicname=<topic-name>
    eventsubscriptionname=<new event subscription name>
    
    az account set --subscription $relayazuresubscriptionid
    relayid=$(az resource show --name $relayname --resource-group $relayrg --resource-type Microsoft.Relay/namespaces --query id --output tsv)
    hybridconnectionid="$relayid/hybridConnections/$hybridname"

    az account set --subscription $topicazuresubscriptionid
    az eventgrid event-subscription create \
      --topic-name $topicname \
      -g $topicresourcegroup \
      --name $eventsubscriptionname \
      --endpoint-type hybridconnection \
      --endpoint $hybridconnectionid

### Installation

- Visual Studio 2017 Version 15.5 or later.

 Clone this repository onto your local machine. Compile the samples inside Visual Studio, the required Microsoft Azure Event Grid SDK components will automatically be downloaded from nuget.org.

 ### Running the Sample

 1. The following are the steps to run the HybridConnectionConsumer sample and see events received on hybrid connection listener:

    a. Load HybridConnectionConsumer project in Visual Studio.

    b. In Program.cs, replace the <relayConnectionString> and <hybridConnectionName> with the relay connection string and hybrid connection name that you created in pre-requisites.

    c. Run this application from Visual Studio to listen for Event Grid Events.

2. Use Event Grid publisher sample(https://github.com/Azure-Samples/event-grid-dotnet-publish-consume-events/tree/master/EventGridPublisher/EventGridPublisher) to publisher events to the topic that you created in pre-requisites.

3. Verify that you received the events in the HybridConnectionConsumer console.
 
## Resources

(Any additional resources or related projects)

- https://docs.microsoft.com/en-us/azure/event-grid/overview
- https://docs.microsoft.com/en-us/azure/event-grid/custom-event-quickstart
- https://github.com/Azure-Samples/event-grid-dotnet-publish-consume-events/tree/master/EventGridPublisher
