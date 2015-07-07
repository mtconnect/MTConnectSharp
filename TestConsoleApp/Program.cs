using MTConnectSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleApp
{
	/// <summary>
	/// Basic example using MTCSharp to stream data from an agent
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{

			var client = new MTConnectClient("http://agent.mtconnect.org");
			client.ProbeCompleted += client_ProbeCompleted;
			client.DataItemChanged += client_DataItemChanged;
			client.DataItemsChanged += client_DataItemsChanged;
			while(true)
			{
				Thread.Sleep(1000);
			}
		}

		static void client_DataItemsChanged(object sender, EventArgs e)
		{
			var client = sender as MTConnectClient;
			Console.WriteLine("Data Items Updated...");
		}

		static void client_DataItemChanged(object sender, DataItemChangedEventArgs e)
		{
			Console.WriteLine("Item Updated: {0}: {1}", e.DataItem, e.DataItem.CurrentSample);
		}

		static void client_ProbeCompleted(object sender, EventArgs e)
		{
			Console.WriteLine("Probe Complete!");
			var client = sender as MTConnectClient;
			client.StartStreaming();
		}
	}
}
