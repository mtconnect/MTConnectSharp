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
	/// Represents a component in the probe response document
	/// </summary>
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Component : MTCItemBase, IComponent
	{
		/// <summary>
		/// The component type
		/// </summary>
		public String Type { get; private set; }

		/// <summary>
		/// Value of the nativeName attribute
		/// </summary>
		public String NativeName { get; private set; }		

		/// <summary>
		/// The DataItems which belong to this component
		/// </summary>
		private List<DataItem> dataItems;

		/// <summary>
		/// The Components which belong to this component
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
		/// Creates a new component
		/// </summary>
		internal Component(XElement xmlComponent) 
		{
			Type = xmlComponent.Name.ToString() ?? String.Empty;
			id = ParseUtility.GetAttribute(xmlComponent, "id");
			Name = ParseUtility.GetAttribute(xmlComponent, "name") == String.Empty ? id : ParseUtility.GetAttribute(xmlComponent, "name");
			NativeName = ParseUtility.GetAttribute(xmlComponent, "nativeName");
			dataItems = ParseUtility.GetDataItems(xmlComponent);
			components = ParseUtility.GetComponents(xmlComponent); 
		}
	}
}
