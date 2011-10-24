namespace Example.Domain
{
    public class StockItem:Scritchy.CQRS.ScratchAR
    {
        int Count = 0;

        public void AddItems(int Count)
        {
            Changes+= new Events.ItemsAdded { StockItemId = Id, Count = Count };
        }

        public void RemoveItems(int Count)
        {
            if (this.Count >= Count)
                Changes += new Events.ItemsRemoved { StockItemId = Id, Count = Count };
        }

        public void OnItemsAdded(int Count)
        {
            this.Count += Count;
        }

        public void OnItemsRemoved(int Count)
        {
            this.Count -= Count;
        }
    }
}
