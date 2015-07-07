using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MTConnectSharp
{
	/// <summary>
	/// Represents a DataItem in the MTConnect probe response
	/// </summary>
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComSourceInterfaces(typeof(IDataItemEvents))]
	public class DataItem : MTCItemBase, IDataItem
	{
		/// <summary>
		/// A new DataItemSample was added to the value buffer
		/// </summary>
		public event EventHandler SampleAdded;

		/// <summary>
		/// DataItemSample collection as a Queue for correct circular buffer behavior
		/// </summary>
		private Queue<DataItemSample> dataItemSamples = new Queue<DataItemSample>();

		/// <summary>
		/// Value of the category attribute
		/// </summary>
		public String Category { get; private set; }

		/// <summary>
		/// Value of the type attribute
		/// </summary>
		public String Type { get; private set; }

		/// <summary>
		/// Value of the subType attribute
		/// </summary>
		public String SubType { get; private set; }

		/// <summary>
		/// Value of the units attribute
		/// </summary>
		public String Units { get; private set; }

		/// <summary>
		/// Value of the nativeUnits attribute
		/// </summary>
		public String NativeUnits { get; private set; }

		/// <summary>
		/// The maximum number of samples to keep in the value buffer
		/// </summary>
		public Int32 BufferSize { get; set; }

		/// <summary>
		/// The value immediately before the value
		/// </summary>
		public DataItemSample PreviousSample
		{
			get
			{
				if (dataItemSamples.Count < 2)
				{
					return null;
				}
				return dataItemSamples.ToList()[dataItemSamples.Count - 2];
			}
		}

		/// <summary>
		/// The current value of this DataItem
		/// </summary>
		public DataItemSample CurrentSample
		{
			get
			{
				return dataItemSamples.Last();
			}
		}

		/// <summary>
		/// Every value in the buffer
		/// </summary>
		public DataItemSample[] SampleHistory
		{
			get
			{
				return dataItemSamples.ToArray();
			}
		}

		/// <summary>
		/// Creates a new DataItem
		/// </summary>
		/// <param name="xmlDataItem">The XElement which defines the DataItem</param>
		internal DataItem(XElement xmlDataItem)
		{
			BufferSize = 100;
			id = ParseUtility.GetAttribute(xmlDataItem, "id");
			Name = ParseUtility.GetAttribute(xmlDataItem, "name");
			Category = ParseUtility.GetAttribute(xmlDataItem, "category");
			Type = ParseUtility.GetAttribute(xmlDataItem, "type");
			SubType = ParseUtility.GetAttribute(xmlDataItem, "subType");
			Units = ParseUtility.GetAttribute(xmlDataItem, "units");
			NativeUnits = ParseUtility.GetAttribute(xmlDataItem, "nativeUnits");
		}

		/// <summary>
		/// Adds a sample to the value buffer and removes the oldest value if the buffer is full
		/// </summary>
		/// <param name="newSample">The new sample to add</param>
		internal void AddSample(DataItemSample newSample)
		{
			dataItemSamples.Enqueue(newSample);
			while(dataItemSamples.Count > BufferSize)
			{
				dataItemSamples.Dequeue();
			}
			sampleAddedHandler();
		}

		private void sampleAddedHandler()
		{
			var args = new EventArgs();
			if(SampleAdded != null)
			{
				SampleAdded(this, args);
			}
		}
	}
}
