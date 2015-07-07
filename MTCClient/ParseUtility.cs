using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MTConnectSharp
{
	internal static class ParseUtility
	{
		/// <summary>
		/// Gets a named attribute from an XElement
		/// </summary>
		/// <param name="xElem">The XElement to parse</param>
		/// <param name="Attribute">The attribute name</param>
		/// <returns>Attribute value or an empty string if not found</returns>
		public static String GetAttribute(XElement xElem, String Attribute)
		{
			if (xElem.Attribute(Attribute) != null)
			{
				return xElem.Attribute(Attribute).Value;
			}
			return String.Empty;
		}

		/// <summary>
		/// Builds a collection of DataItems from an XElement
		/// </summary>
		/// <param name="xElem">The XElement to parse</param>
		/// <returns>A Collection of DataItems</returns>
		public static List<DataItem> GetDataItems(XElement xElem)
		{
			List<DataItem> rtnList = new List<DataItem>();
			if (xElem.Elements().Any(e => e.Name.LocalName == "DataItems"))
			{
				XElement xmlDataItems = xElem.Elements().First(e => e.Name.LocalName == "DataItems");
				if (xmlDataItems != null) // DataItems Exist
				{
					foreach (var xmlDataItem in xmlDataItems.Elements())
					{
						rtnList.Add(new DataItem(xmlDataItem));
					}
				}
			}
			return rtnList;
		}

		/// <summary>
		/// Builds a collection of Components from an XElement
		/// </summary>
		/// <param name="xElem">The XElement to parse</param>
		/// <returns>A Collection of Components</returns>
		public static List<Component> GetComponents(XElement xElem)
		{
			List<Component> rtnList = new List<Component>();
			if (xElem.Elements().Any(e => e.Name.LocalName == "Components"))
			{
				XElement xmlComponents = xElem.Elements().First(e => e.Name.LocalName == "Components");
				if (xmlComponents != null) // Components Exist
				{
					foreach (XElement xmlComponent in xmlComponents.Elements())
					{
						rtnList.Add(new Component(xmlComponent));
					}
				}
			}
			return rtnList;
		}
	}
}
