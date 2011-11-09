using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Commands;
using Example.Domain.Readmodel;
using Newtonsoft.Json;
using Scritchy.Infrastructure.Configuration;
using SignalR.Hubs;
using Scritchy.Infrastructure;

namespace Example.SignalR.Hubs
{
    public class StockHub: Hub
    {
        ScritchyBus bus;
        StockDictionary dict;
        static Dictionary<string, object> CommandExamples = new Dictionary<string, object>();

        public StockHub(ScritchyBus bus,StockDictionary dict)
        {
            InitializeCommands();
            this.bus = bus;
            this.dict = dict;
            bus.ApplyNewEventsToAllHandlers();
        }

        public void Initialize()
        {
            Caller.updateCommandList(CommandExamples.Keys); 
            Clients.updateStockList(dict);
        }

        public void LoadCommandTemplate(string fulltypename)
        {
            Caller.updateCommandEntry(CommandExamples[fulltypename??CommandExamples.Keys.First()]);
        }

        public void RunCommand(string fulltypename,string serializeddata)
        {
            var t = Type.GetType(fulltypename);
            var obj = JsonConvert.DeserializeObject(serializeddata, t);

            bus.RunCommandAsync(obj, rwce => { 
                if (rwce.Error == null)
                    Clients.updateStockList(dict);
                else
                    Caller.alert(rwce.Error.Message);
            });
        }


        void InitializeCommands()
        {
            lock (CommandExamples)
            {
                if (CommandExamples.Count > 0) return;
                foreach (var cmd in new object[]{
                    new AllowItem {StockItemId = "Item/1",Name = "Item 1"},
                    new BanItem {StockItemId = "Item/1"},
                    new AddItems {StockItemId = "Item/1",Amount = 3},
                    new RemoveItems {StockItemId = "Item/1",Amount = 3}
                })
                {
                    CommandExamples.Add(cmd.GetType().AssemblyQualifiedName, cmd);
                }
            }
        }

    }
}