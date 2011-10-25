using System.Collections.Generic;
using System.Linq;

namespace Example.Domain.Readmodel
{
    public class StockListItem
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }

    public class StockDictionary : Dictionary<string, StockListItem> { }

    public class StockDictionaryHandler
    {
        StockDictionary instance;

        public StockDictionaryHandler(StockDictionary instance)
        {
            this.instance = instance;
        }

        public void OnItemAllowed(string StockItemId, string Name)
        {
            instance.Add(StockItemId, new StockListItem { Name = Name, Amount = 0 });
        }

        public void OnItemBanned(string StockItemId)
        {
            instance.Remove(StockItemId);
        }

        public void OnItemsAdded(string StockItemId, int Amount)
        {
            instance[StockItemId].Amount += Amount;
        }

        public void OnItemsRemoved(string StockItemId, int Amount)
        {
            instance[StockItemId].Amount -= Amount;
        }



    }
}
