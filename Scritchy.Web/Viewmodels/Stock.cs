using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scritchy.Web.Viewmodels
{
    public class StockList
    {
        Dictionary<string, int> Items = new Dictionary<string,int>();

        public void OnItemAdded(string StockItemId, int Count)
        {
            if (Items.ContainsKey(StockItemId))
                Items[StockItemId] += Count;
            else
                Items[StockItemId] = Count;
        }

        public void OnItemRemoved(string StockItemId, int Count)
        {
            if (Items.ContainsKey(StockItemId))
                Items[StockItemId] -= Count;
            if (Items[StockItemId] == 0)
                Items.Remove(StockItemId);
        }
    }
}