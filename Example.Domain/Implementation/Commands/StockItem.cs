namespace Example.Domain.Implementation.Commands
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
