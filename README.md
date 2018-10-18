# MTConnectSharp
A Simple MTConnect C# Client Library

Dependencies: RestSharp open source C# REST client library, referenced in the package.config file.

This library was originally written to monitor individual data items as they changed to better facilitate rule development in the early days of MTconnect. As such it was written to be as simple as possible, so it does not support much beyond simple probe parsing and quasi-streaming the CDATA of dataitem tags as they change.

The TestConsoleApp project provides a basic example of streaming data from agent.mtconnect.org.

Because MTConnect is an HTTP protocol, this library handles all communication asynchronously. There are events which an application can subscribe to and react accordingly after successful responses.

## Usage Example

The entry point is the `MTConnectClient` class, which can be initialized via the constructor:

	var client = new MTConnectSharp.MTConnectClient()
	{
		AgentUri = "http://agent.mtconnect.org",
		UpdateInterval = TimeSpan.FromSeconds(.5)
	};

The above code block initializes the client, probes the agent and populate the `Devices` collection of the `MTConnectClient`. When the probe parsing is complete the `ProbeCompleted` event fires.

The `Devices`  collection is observable and may contain `Component`s and `DataItem`s, and `Component`s may contain `Component`s and `DataItem`s, but `DataItem`s do not have child `DataItem`s or `Component`s, only `DataItemSample`s, which provide `Value`s and `Timestamp`s. All collections are exposed readonly observable collections to prevent modification and support change notification.

After the probe is complete, we can start reading data items. There are two ways to initiate a data pull from the Agent: current and sample. `GetCurrentState()` sends a current command and processes the data, and `StartStreaming()` begins with a current, then requests samples at the `UpdateInterval` to get values for any DataItems which have changed.

**Do not use repeated `GetCurrentState()` calls for streaming. It will burden the Agent and the client unneccessarily.**

The `ReadOnlyObservableCollection<DataItem>` members of `Device` and `Component` will fire a `CollectionChanged` event when a new `DataItemSample` is added to or removed from its buffer.

## Known Issues ##

* There are no tests. This project needs tests before further refactoring and features can be added.
* Since this version is using the Net Core framework, there is no support for COM/VBA.
* Potential error points do not have exception wrappers to help identify them when debugging.

## Future Enhancements ##

* Add support for detailed events and conditions separate from sample-based data items
* Support assets and asset change events
