using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTConnectSharp
{
	/// <summary>
	/// Data for the DataItemChanged Event
	/// </summary>
	[ComVisibleAttribute(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	public class DataItemChangedEventArgs : EventArgs, IDataItemChangedEventArgs
	{
		/// <summary>
		/// DataItem that was changed
		/// </summary>
		public DataItem DataItem { get; set; }

		/// <summary>
		/// Class Constructor
		/// </summary>
		/// <param name="dataItem">The DataItem that was changed</param>
		internal DataItemChangedEventArgs(DataItem dataItem)
		{
			DataItem = dataItem;
		}
	}
}
