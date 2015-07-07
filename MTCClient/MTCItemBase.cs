using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTConnectSharp
{
	/// <summary>
	/// The base MTConnect Item Class
	/// </summary>
	[ComVisible(false)]
	public abstract class MTCItemBase
	{
		/// <summary>
		/// Value of the id attribute
		/// </summary>
		public String id { get; internal set; }

		/// <summary>
		/// Value of the name attribute
		/// </summary>
		public String Name { get; internal set; }

		/// <summary>
		/// Name and id of the item as a formatted string
		/// </summary>
		public String LongName
		{
			get
			{
				return String.Format("{0}({1})", this.Name, this.id);
			}
		}

		/// <summary>
		/// Formatted string describing the item
		/// </summary>
		public override string ToString()
		{
			try
			{
				return String.Format("{0}({1})", this.Name, this.id);
			}
			catch
			{
				return base.ToString();
			}
		}
	}
}
