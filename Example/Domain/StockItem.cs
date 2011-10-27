namespace Example.Domain
{
    public class StockItem : Scritchy.Domain.AR
    {
        int Amount = 0;
        bool IsAllowed = false;

        public void AllowItem(string Name)
        {
            if (IsAllowed == true) return;
            Changes += new Events.ItemAllowed { StockItemId = Id, Name = Name };
        }

        public void BanItem()
        {
            if (IsAllowed == false) return;
            Guard.Against(Amount > 0, "An item that is in stock can not be banned");
            Changes += new Events.ItemBanned { StockItemId = Id };
        }

        public void AddItems(int Amount)
        {
            Guard.Against(IsAllowed == false, "An item of this type is not allowed in the stock");
            Changes += new Events.ItemsAdded { StockItemId = Id, Amount = Amount };
        }

        public void RemoveItems(int Amount)
        {
            Guard.Against(IsAllowed == false, "An item of this type is not allowed in the stock");
            Guard.Against(this.Amount < Amount, "You do not have enough stock left to remove this amount of items");
            Changes += new Events.ItemsRemoved { StockItemId = Id, Amount = Amount };
        }

        public void OnItemsAdded(int Amount)
        {
            this.Amount += Amount;
        }

        public void OnItemsRemoved(int Amount)
        {
            this.Amount -= Amount;
        }

        public void OnItemAllowed()
        {
            IsAllowed = true;
        }

        public void OnItemBanned()
        {
            IsAllowed = false;
        }
    }
}
