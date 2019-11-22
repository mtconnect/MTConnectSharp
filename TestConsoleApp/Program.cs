using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Terminal.Gui;

namespace TestConsoleApp
{
   /// <summary>
   /// Basic example using MTCSharp to stream data from an agent
   /// </summary>
   class Program
   {
      static HashSet<INotifyCollectionChanged> Registered = new HashSet<INotifyCollectionChanged>();

      static Dictionary<Type, Action<object>> Hellos = new Dictionary<Type, Action<object>>();
      static Dictionary<Type, Action<object>> Goodbyes = new Dictionary<Type, Action<object>>();

      static void AddHelloGoodbye<T>(Action<T> hello, Action<T> goodbye)
      {
         Hellos.Add(typeof(T), o => hello((T)o));
         Goodbyes.Add(typeof(T), o => goodbye((T)o));
      }

      static Dictionary<Type, NotifyCollectionChangedEventHandler> Handlers = 
         new Dictionary<Type, NotifyCollectionChangedEventHandler>();

      static NotifyCollectionChangedEventHandler MakeHandler<T>()
      {
         var t = typeof(T);
         if (!Handlers.ContainsKey(t))
         {
            Handlers.Add(t, (sender, info) =>
            {
               switch (info.Action)
               {
                  case NotifyCollectionChangedAction.Add:
                  case NotifyCollectionChangedAction.Remove:
                  case NotifyCollectionChangedAction.Replace:
                  case NotifyCollectionChangedAction.Reset:
                     if (info.NewItems != null)
                     {
                        foreach (T item in info.NewItems)
                        {
                           Hellos[typeof(T)](item);
                        }
                     }
                     if (info.OldItems != null)
                     {
                        foreach (T item in info.OldItems)
                        {
                           Goodbyes[typeof(T)](item);
                        }
                     }
                     break;

                  case NotifyCollectionChangedAction.Move:
                     break;
               }
            });
         }

         return Handlers[t];
      }

      static void Attach<T>(ReadOnlyObservableCollection<T> collection)
      {
         if (!Registered.Contains(collection))
         {
            Registered.Add(collection);
            (collection as INotifyCollectionChanged).CollectionChanged += MakeHandler<T>();
         }
         foreach (var t in collection)
         {
            Hellos[typeof(T)](t);
         }
      }

      static void Detach<T>(ReadOnlyObservableCollection<T> collection)
      {
         if (Registered.Contains(collection))
         {
            Registered.Remove(collection);
            (collection as INotifyCollectionChanged).CollectionChanged -= MakeHandler<T>();
         }
         foreach (var t in collection)
         {
            Goodbyes[typeof(T)](t);
         }
      }

      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");
         var client = new MTConnectSharp.MTConnectClient()
         {
            AgentUri = "http://agent.mtconnect.org",
            UpdateInterval = TimeSpan.FromSeconds(.5)
         };

         client.ProbeCompleted += (sender, info) => {
            var items = client.Devices
               .SelectMany(d => d.DataItems.Select(i => new { d = d.LongName, i = i.LongName }))
               .ToArray();

            Console.WriteLine($"Number of DataItems: {items.Count()}");

            client.StartStreaming();
         };

         AddHelloGoodbye<MTConnectSharp.Device>(
            d => { Attach(d.Components); Attach(d.DataItems); }, 
            d => { Detach(d.Components); Detach(d.DataItems); }
         );
         AddHelloGoodbye<MTConnectSharp.Component>(
            c => { Attach(c.Components); Attach(c.DataItems); }, 
            c => { Detach(c.Components); Detach(c.DataItems); }
         );
         AddHelloGoodbye<MTConnectSharp.DataItem>(
            i => Attach(i.SampleHistory), 
            i => Detach(i.SampleHistory)
         );
         var samples = 0L;
         AddHelloGoodbye<MTConnectSharp.DataItemSample>(
            s => samples++,
            s => samples--
         );

         /* UI */
         Application.Init();
         var top = Application.Top;
         var win = new Window(new Rect(0, 1, top.Frame.Width, top.Frame.Height - 1), "MyApp");
         top.Add(win);

         var count = new Label(14, 1, "     ");
         win.Add(new Label(3, 1, "Samples:"), count);
         Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(.25), m => {
            count.Text = samples.ToString();
            return true;
         });         

         Attach(client.Devices);
         client.Probe();

         Application.Run();
      }
   }
}
