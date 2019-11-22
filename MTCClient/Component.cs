using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace MTConnectSharp
{
   /// <summary>
   /// Represents a component in the probe response document
   /// </summary>
   public class Component : MTCItemBase, IComponent
	{
		/// <summary>
		/// The component type
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Value of the nativeName attribute
		/// </summary>
		public string NativeName { get; set; }		

		/// <summary>
		/// The DataItems which belong to this component
		/// </summary>
		private ObservableCollection<DataItem> _dataItems;

		/// <summary>
		/// The Components which belong to this component
		/// </summary>
		private ObservableCollection<Component> _components;

		/// <summary>
		/// Array of the DataItems collection for COM Interop
		/// </summary>
		public ReadOnlyObservableCollection<DataItem> DataItems
		{
         get; private set;
		}

		/// <summary>
		/// Array of the Components collection for COM Interop
		/// </summary>
		public ReadOnlyObservableCollection<Component> Components
		{
         get;
         private set;
		}

		/// <summary>
		/// Creates a new component
		/// </summary>
		internal Component(XElement xmlComponent) 
		{
         Type = xmlComponent.Name?.ToString() ?? string.Empty;
         Id = ParseUtility.GetAttribute(xmlComponent, "id");
         Name = ParseUtility.GetAttribute(xmlComponent, "name");
         if (string.IsNullOrEmpty(Name))
            Name = Id;
         NativeName = ParseUtility.GetAttribute(xmlComponent, "nativeName");

         _dataItems = new ObservableCollection<DataItem>(ParseUtility.GetDataItems(xmlComponent));
         DataItems = new ReadOnlyObservableCollection<DataItem>(_dataItems);

         _components = new ObservableCollection<Component>(ParseUtility.GetComponents(xmlComponent));
         Components = new ReadOnlyObservableCollection<Component>(_components);
		}
	}
}
