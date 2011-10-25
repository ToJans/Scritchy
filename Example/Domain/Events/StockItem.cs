
namespace Example.Domain.Events
{
    public class ItemAllowed
    {
        public string StockItemId { get; set; }
        public string Name { get; set; }
    }

    public class ItemBanned
    {
        public string StockItemId { get; set; }
    }

    public class ItemsRemoved
    {
        public string StockItemId { get; set; }
        public int Amount { get; set; }
    }

    public class ItemsAdded
    {
        public string StockItemId { get; set; }
        public int Amount { get; set; }
    }
}
