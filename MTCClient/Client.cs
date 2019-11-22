using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Xml.Linq;

namespace MTConnectSharp
{
   /// <summary>
   /// Connects to a single agent and streams data from it.
   /// </summary>
   public class MTConnectClient : IMTConnectClient, IDisposable
    {
		/// <summary>
		/// The probe response has been recieved and parsed
		/// </summary>
		public event EventHandler ProbeCompleted;

      /// <summary>
      /// The base uri of the agent
      /// </summary>
      public string AgentUri { get; set; }

		/// <summary>
		/// Time in milliseconds between sample queries when simulating a streaming connection
		/// </summary>
		public TimeSpan UpdateInterval { get; set; }

		/// <summary>
		/// Devices on the connected agent
		/// </summary>
		public ReadOnlyObservableCollection<Device> Devices
		{
         get;
         private set;
		}
		private ObservableCollection<Device> _devices;

		/// <summary>
		/// Dictionary Reference to all data items by id for better performance when streaming
		/// </summary>
		private Dictionary<string, DataItem> _dataItemsDictionary = new Dictionary<string,DataItem>(); 
		
		/// <summary>
		/// RestSharp RestClient
		/// </summary>
		private RestClient _restClient;
		
		/// <summary>
		/// Not actually parsing multipart stream - this timer fires sample queries to simulate streaming
		/// </summary>
		private Timer _streamingTimer;

		/// <summary>
		/// Last sequence number read from current or sample
		/// </summary>
		private long _lastSequence;

      private bool _probeStarted = false;
      private bool _probeCompleted = false;

		/// <summary>
		/// Initializes a new Client 
		/// </summary>
		public MTConnectClient()
		{
			UpdateInterval = TimeSpan.FromMilliseconds(2000);

         _devices = new ObservableCollection<Device>();
         Devices = new ReadOnlyObservableCollection<Device>(_devices);
		}

		/// <summary>
		/// Starts sample polling and updating DataItem values as they change
		/// </summary>
		public void StartStreaming()
		{
			if (_streamingTimer?.Enabled == true)
			{
				return;
			}

			GetCurrentState();

			_streamingTimer = new Timer(UpdateInterval.TotalMilliseconds);
			_streamingTimer.Elapsed += StreamingTimerElapsed;
			_streamingTimer.Start();
		}

		/// <summary>
		/// Stops sample polling
		/// </summary>
		public void StopStreaming()
		{
			_streamingTimer.Stop();
		}

		/// <summary>
		/// Gets current response and updates DataItems
		/// </summary>
		public void GetCurrentState()
		{
			if (!_probeCompleted)
			{
				throw new InvalidOperationException("Cannot get DataItem values. Agent has not been probed yet.");
			}

         var request = new RestRequest
         {
            Resource = "current"
         };
         _restClient.ExecuteAsync(request, (a) => ParseStream(a));
		}

		/// <summary>
		/// Gets probe response from the agent and populates the devices collection
		/// </summary>
		public void Probe()
		{
         if (_probeStarted && !_probeCompleted)
         {
            throw new InvalidOperationException("Cannot start a new Probe when one is still running.");
         }

         _restClient = new RestClient
         {
            BaseUrl = new Uri(AgentUri)
         };

         var request = new RestRequest
         {
            Resource = "probe"
         };

         try
			{
            _probeStarted = true;
				_restClient.ExecuteAsync(request, r => ParseProbeResponse(r));
			}
			catch (Exception ex)
			{
            _probeStarted = false;
            throw new Exception("Probe request failed.\nAgent Uri: " + AgentUri, ex);
         }
      }

		/// <summary>
		/// Parses IRestResponse from a probe command into a Device collection
		/// </summary>
		/// <param name="response">An IRestResponse from a probe command</param>
		private void ParseProbeResponse(IRestResponse response)
		{
         var xdoc = XDocument.Load(new StringReader(response.Content));
         if (_devices.Any())
            _devices.Clear();

         _devices.AddRange(xdoc.Descendants()
            .Where(d => d.Name.LocalName == "Devices")
            .Take(1) // needed? 
            .SelectMany(d => d.Elements())
            .Select(d => new Device(d)));

         BuildDataItemDictionary();

			_probeCompleted = true;
         _probeStarted = false;
			ProbeCompletedHandler();			
		}

		/// <summary>
		/// Loads DataItemRefList with all data items from all devices
		/// </summary>
		private void BuildDataItemDictionary()
		{
         _dataItemsDictionary = _devices.SelectMany(d =>
            d.DataItems.Concat(GetAllDataItems(d.Components))
         ).ToDictionary(i => i.Id, i => i);
		}

		/// <summary>
		/// Recursive function to get DataItems list from a Component collection
		/// </summary>
		/// <param name="components">Collection of Components</param>
		/// <returns>Collection of DataItems from passed Component collection</returns>
		private static List<DataItem> GetAllDataItems(IReadOnlyList<Component> components)
		{
         var queue = new Queue<Component>(components);
			var dataItems = new List<DataItem>();
			while(queue.Count > 0)
			{
            var component = queue.Dequeue();
            foreach (var c in component.Components)
               queue.Enqueue(c);
            dataItems.AddRange(component.DataItems);
			}
			return dataItems;
		}

		private void StreamingTimerElapsed(object sender, ElapsedEventArgs e)
		{
         var request = new RestRequest
         {
            Resource = "sample"
         };
         request.AddParameter("at", _lastSequence + 1);
			_restClient.ExecuteAsync(request, r => ParseStream(r));
		}

		/// <summary>
		/// Parses response from a current or sample request, updates changed data items and fires events
		/// </summary>
		/// <param name="response">IRestResponse from the MTConnect request</param>
		private void ParseStream(IRestResponse response)
		{
         using (StringReader sr = new StringReader(response.Content))
         {
            var xdoc = XDocument.Load(sr);

            _lastSequence = Convert.ToInt64(xdoc.Descendants().First(e => e.Name.LocalName == "Header")
               .Attribute("lastSequence").Value);

            var xmlDataItems = xdoc.Descendants()
               .Where(e => e.Attributes().Any(a => a.Name.LocalName == "dataItemId"));
            if (xmlDataItems.Any())
            {
               var dataItems = xmlDataItems.Select(e => new {
                  id = e.Attribute("dataItemId").Value,
                  timestamp = DateTime.Parse(e.Attribute("timestamp").Value, null, 
                     System.Globalization.DateTimeStyles.RoundtripKind),
                  value = e.Value
               })
               .OrderBy(i => i.timestamp)
               .ToList();

               foreach (var item in dataItems)
               {
                  var dataItem = _dataItemsDictionary[item.id];
                  var sample = new DataItemSample(item.value.ToString(), item.timestamp);
                  dataItem.AddSample(sample);
               }
            }
         }
      }

      private void ProbeCompletedHandler()
		{
			ProbeCompleted?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Disposes unmanaged resources
		/// </summary>
		public void Dispose()
		{
			_streamingTimer?.Dispose();
		}
	}
}
