using System;
namespace MTConnectSharp
{
	public interface IDataItem
	{
		int                BufferSize     { get; set; }
		DataItemSample    PreviousSample  { get; }
		DataItemSample    CurrentSample          { get; }
		DataItemSample[]  SampleHistory   { get; }
		String             id             { get; }
		String             Name           { get; }
		String             LongName       { get; }
		String             Category       { get; }
		String             Type           { get; }
		String             SubType        { get; }
		String             Units          { get; }
		String             NativeUnits    { get; }
	}
}
