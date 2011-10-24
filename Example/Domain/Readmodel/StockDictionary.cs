using System.Collections.Generic;

namespace Example.Domain.Readmodel 
{
    public class StockDictionary : Dictionary<string, int> { }

    public class StockDictionaryHandler
    {
        StockDictionary instance;

        public StockDictionaryHandler(StockDictionary instance)
        {
            this.instance = instance;
        }

        public void OnItemsAdded(string StockItemId, int Count)
        {
            VerifyExists(StockItemId);
            instance[StockItemId] += Count;
        }

        public void OnItemsRemoved(string StockItemId, int Count)
        {
            VerifyExists(StockItemId);
            instance[StockItemId] -= Count;
            if (instance[StockItemId] == 0)
                instance.Remove(StockItemId);
        }

        void VerifyExists(string StockItemId)
        {
            if (!instance.ContainsKey(StockItemId))
                instance.Add(StockItemId, 0);
        }

    
    }
}
