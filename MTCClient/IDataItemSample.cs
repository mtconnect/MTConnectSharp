using System;

namespace MTConnectSharp
{
	public interface IDataItemSample
	{
		DateTime TimeStamp { get; }
		string ToString();
		string Value { get; }
	}
}
