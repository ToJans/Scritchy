
namespace Example.Domain.Implementation.Events
{
    public class ItemsRemoved
    {
        public string StockItemId { get; set; }
        public int Count { get; set; }
    }

    public class ItemsAdded
    {
        public string StockItemId { get; set; }
        public int Count { get; set; }
    }
}
