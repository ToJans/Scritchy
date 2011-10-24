namespace Example.Domain.Commands
{
    public class AddItems
    {
        public string StockItemId { get; set; }
        public int Count { get; set; }
    }

    public class RemoveItems
    {
        public string StockItemId { get; set; }
        public int Count { get; set; }
    }
}
