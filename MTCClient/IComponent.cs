using System.Collections.ObjectModel;

namespace MTConnectSharp
{
   public interface IComponent
	{
      ReadOnlyObservableCollection<Component> Components { get; }
      ReadOnlyObservableCollection<DataItem> DataItems { get; }
		string Type { get; }
		string Id { get; }
		string Name { get; }
		string LongName { get; }
	}
}
