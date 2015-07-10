# MTConnectSharp
A Simple MTConnect C# Client Library

Dependencies: RestSharp open source C# REST client library, referenced in the package.config file.

This library was originally written to monitor individual data items as they changed to better facilitate rule development in the early days of MTconnect. As such it was written to be as simple as possible, so it does not support much beyond simple probe parsing and quasi-streaming the CDATA of dataitem tags as they change.

The TestConsoleApp project provides a basic example of streaming data from agent.mtconnect.org.

Because MTConnect is an HTML protocol, this library handles all communication asynchronously. There are events which an application can subscribe to and react accordingly after successful responses.

##Usage Example

The entry point is the MTConnectClient, which can be initialized with or without a uri string in the constructor:

    var client = new MTConnectClient("http://agent.mtconnect.org");

or

    var client = new MTConnectClient();
    client.AgentUri = "http://agent.mtconnect.org";
    client.Probe();

Both of the above code blocks do the same thing. They initialize the client, probe the agent, and populate the `Devices` collection of the MTConnectClient. When the probe parsing is complete the `ProbeCompleted` event fires.

Devices may contain Components and DataItems, and Components may contain Components and DataItems, but DataItems do not have child DataItems or Components, only DataItemSamples, which provide Values and Timestamps. All collections are exposed arrays to prevent modification and support COM interfacing.

After the probe is complete, we can start reading data items. There are two ways to initiate a data pull from the Agent: current and sample. `GetCurrentState()` sends a current command and processes the data, and `StartStreaming()` begins with a current, then requests samples at the `UpdateInterval` to get values for any DataItems which have changed.

**Do not use repeated `GetCurrentState()` calls for streaming. It will burden the Agent and the client unneccessarily.**

There are three different ways to watch for new DataItemSamples added to a DataItem. The MTConnectClient will fire a `DataItemChanged` event for each DataItem when a new DataItemSample is added to its buffer. The MTConnectClient will fire a `DataItemsChanged` event after it completes processing of a current or sample request that updates any DataItems. Also the DataItem will fire a 'SampleAdded' event when a new DataItemSample is added, to facilitate subscribing directly to a specific DataItem to watch it for changes.

##Known Issues##

* There are no tests. This project needs tests before further refactoring and features can be added.
* Nested arrays are not exposing to VBA(COM) correctly. There is not much information out there to help diagnose or resolve this. There are several SO questions related to this problem either directly or indirectly, but no working solutions that I've found.
* MTConnectClient.DataItemChanged event does not trigger from VBA(COM) interface. It is exposed, but never actually fires.
* Potential error points do not have exception wrappers to help identify them when debugging.

##Future Enhancements##

* Add support for detailed events and conditions separate from sample-based data items
* Support assets and asset change events
