using System.Collections.ObjectModel;

namespace MTConnectSharp
{
   public interface IDevice
	{
      ReadOnlyObservableCollection<Component> Components { get; }
      ReadOnlyObservableCollection<DataItem> DataItems { get; }
		string Description { get; }
		string Manufacturer { get; }
		string SerialNumber { get; }
		string Id { get; }
		string Name { get; }
		string LongName { get; }
	}
}
