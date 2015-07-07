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
	/// Represents a device in the MTConnect probe response
	/// </summary>
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Device : MTCItemBase, IDevice
	{

		/// <summary>
		/// Description of the device
		/// </summary>
		public String Description { get; private set; }

		/// <summary>
		/// Manufacturer of the device
		/// </summary>
		public String Manufacturer { get; private set; }

		/// <summary>
		/// Serial number of the device
		/// </summary>
		public String SerialNumber { get; private set; }

		/// <summary>
		/// The DataItems which are direct children of the device
		/// </summary>
		private List<DataItem> dataItems;

		/// <summary>
		/// The components which are direct children of the device
		/// </summary>
		private List<Component> components;

		/// <summary>
		/// Array of the DataItems collection for COM Interop
		/// </summary>
		public DataItem[] DataItems
		{ 
			get
			{
				return dataItems.ToArray();	
			}
		}

		/// <summary>
		/// Array of the Components collection for COM Interop
		/// </summary>
		public Component[] Components
		{
			get
			{
				return components.ToArray();
			}
		}

		/// <summary>
		/// Creates a new device from an MTConnect XML device node
		/// </summary>
		/// <param name="xElem">The MTConnect XML node which defines the device</param>
		internal Device(XElement xElem)
		{
			if (xElem.Name.LocalName == "Device")
			{
				// Populate basic fields
				id = ParseUtility.GetAttribute(xElem, "id");
				Name = ParseUtility.GetAttribute(xElem, "name");
				XElement desc = xElem.Descendants().First(x => x.Name.LocalName == "Description");
				Description = desc.Value ?? String.Empty;
				Manufacturer = ParseUtility.GetAttribute(desc, "manufacturer");
				SerialNumber = ParseUtility.GetAttribute(desc, "serialNumber");
				dataItems = ParseUtility.GetDataItems(xElem);
				components = ParseUtility.GetComponents(xElem);
			}
		}

	}
}
