using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MTConnectSharp
{
   internal static class ParseUtility
	{
      public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
      {
         foreach (var i in items) collection.Add(i);
      }

      public static void RemoveRange<T>(this ObservableCollection<T> collection, int start, int count)
      {
         for(var i = 0; i < count && i < collection.Count; ++i)
            collection.RemoveAt(start);
      }

      /// <summary>
      /// Gets a named attribute from an XElement
      /// </summary>
      /// <param name="xElem">The XElement to parse</param>
      /// <param name="Attribute">The attribute name</param>
      /// <returns>Attribute value or an empty string if not found</returns>
      public static string GetAttribute(XElement xElem, string Attribute)
		{
			if (xElem.Attribute(Attribute) != null)
			{
				return xElem.Attribute(Attribute).Value;
			}
			return string.Empty;
		}

		/// <summary>
		/// Builds a collection of DataItems from an XElement
		/// </summary>
		/// <param name="xElem">The XElement to parse</param>
		/// <returns>A Collection of DataItems</returns>
		public static List<DataItem> GetDataItems(XElement xElem)
		{
         return xElem.Elements()
            .Where(e => e.Name.LocalName == "DataItems")
            .Take(1) // needed?
            .SelectMany(d => d.Elements())
            .Select(d => new DataItem(d))
            .ToList();
		}

		/// <summary>
		/// Builds a collection of Components from an XElement
		/// </summary>
		/// <param name="xElem">The XElement to parse</param>
		/// <returns>A Collection of Components</returns>
		public static List<Component> GetComponents(XElement xElem)
		{
         return xElem.Elements()
            .Where(e => e.Name.LocalName == "Components")
            .Take(1) // needed?
            .SelectMany(d => d.Elements())
            .Select(d => new Component(d))
            .ToList();
		}
	}
}
