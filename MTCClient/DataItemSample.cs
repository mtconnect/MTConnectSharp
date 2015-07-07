using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTConnectSharp
{
	/// <summary>
	/// A single value from a current or sample response
	/// </summary>
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class DataItemSample : MTConnectSharp.IDataItemSample
	{
		/// <summary>
		/// The value of the sample
		/// </summary>
		public String Value { get; private set; }

		/// <summary>
		/// The timestamp of the sample
		/// </summary>
		public DateTime TimeStamp { get; private set; }

		/// <summary>
		/// Creates a new sample with the current time as the timestamp
		/// </summary>
		/// <param name="value">Value of the sample</param>
		internal DataItemSample(String value)
		{
			TimeStamp = DateTime.Now;
			Value = value;
		}

		/// <summary>
		/// Creates a new sample
		/// </summary>
		/// <param name="value">Value of the sample</param>
		/// <param name="timestamp">Timestamp of the sample</param>
		internal DataItemSample(String value, DateTime timestamp)
		{
			TimeStamp = timestamp;
			Value = value;
		}

		/// <summary>
		/// Returns the Value
		/// </summary>
		public override string ToString()
		{
			return Value;
		}
	}

}

