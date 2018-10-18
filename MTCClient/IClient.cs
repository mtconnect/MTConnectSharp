using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTConnectSharp
{
   public interface IMTConnectClient
	{
		string AgentUri { get; set; }
		void Probe();
		void StartStreaming();
		void StopStreaming();
		void GetCurrentState();
		ReadOnlyObservableCollection<Device> Devices { get; }
		TimeSpan UpdateInterval { get; set; }
	}
}
