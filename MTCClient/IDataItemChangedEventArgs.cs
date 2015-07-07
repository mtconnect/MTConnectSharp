using System;
using System.Runtime.InteropServices;
namespace MTConnectSharp
{
	public interface IDataItemChangedEventArgs
	{
		DataItem DataItem { get; set; }
	}
}
