namespace Example.Domain.Commands
{
    public class AddItems
    {
        public string StockItemId { get; set; }
        public int Amount { get; set; }
    }

    public class RemoveItems
    {
        public string StockItemId { get; set; }
        public int Amount { get; set; }
    }
    
    public class AllowItem
    {
        public string StockItemId { get; set; }
        public string Name { get; set; }
    }

    public class BanItem
    {
        public string StockItemId { get; set; }
    }
}
