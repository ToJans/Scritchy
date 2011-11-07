using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Commands;
using Example.Domain.Readmodel;
using Newtonsoft.Json;
using Scritchy.Infrastructure.Configuration;
using SignalR.Hubs;

namespace Example.SignalR.Hubs
{
    public class StockHub: Hub
    {
        ScritchyBus bus;
        StockDictionary dict;
        static Dictionary<string, string> CommandsInJson = new Dictionary<string, string>();

        public StockHub(ScritchyBus bus,StockDictionary dict)
        {
            InitializeCommandsInJson();
            this.bus = bus;
            this.dict = dict;
            bus.ApplyNewEventsToAllHandlers();
        }

        public void Initialize()
        {
            Caller.updateCommandList(CommandsInJson.Keys); 
            Clients.updateStockList(dict);
        }

        public void LoadCommandTemplate(string fulltypename)
        {
            Caller.updateCommandEntry(CommandsInJson[fulltypename??CommandsInJson.Keys.First()]);
        }

        /*
        class TypedConverter<T>
        {
            public object ConvertFromJson(string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }*/

        public void RunCommand(string fulltypename,string serializeddata)
        {
            var t = Type.GetType(fulltypename);
            /*
            dynamic cvtinst = Activator.CreateInstance(typeof(TypedConverter<>).MakeGenericType(t));
            object obj = cvtinst.ConvertFromJson(serializeddata);
             */
            var obj = JsonConvert.DeserializeObject(serializeddata, t);
            try
            {
                bus.RunCommand(obj);
                Clients.updateStockList(dict);
            }
            catch (Exception e)
            {
                Caller.alert(e.Message);
            }
        }

        void InitializeCommandsInJson()
        {
            lock (CommandsInJson)
            {
                if (CommandsInJson.Count > 0) return;
                foreach (var cmd in new object[]{
                    new AllowItem {StockItemId = "Item/1",Name = "Item 1"},
                    new BanItem {StockItemId = "Item/1"},
                    new AddItems {StockItemId = "Item/1",Amount = 3},
                    new RemoveItems {StockItemId = "Item/1",Amount = 3}
                })
                {
                    CommandsInJson.Add(cmd.GetType().AssemblyQualifiedName, JsonConvert.SerializeObject(cmd));
                }
            }
        }

    }
}