using System.Linq;
using Example.Domain.Implementation.Commands;
using Example.Domain.Implementation.Events;
using Machine.Specifications;

namespace Example.Specs
{
    public class add_items_to_stock : _in_stockcontext
    {
        Because of =
            () => ApplyCommand(new AddItems { StockItemId = ItemId, Count = 5 });

        It should_have_added_the_count_to_the_stock =
            () => NewEvents.OfType<ItemsAdded>().ShouldContain(x => x.StockItemId == ItemId && x.Count == 5);
    }

    public class remove_small_amount_from_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(new ItemsAdded { StockItemId = ItemId, Count = 10 });

        Because of =
            () => ApplyCommand(new RemoveItems { StockItemId = ItemId, Count = 5 });

        It should_have_removed_the_count_from_the_stock =
            () => NewEvents.OfType<ItemsRemoved>().ShouldContain(x => x.StockItemId == ItemId && x.Count == 5);
    }

    public class remove_to_much_from_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(new ItemsAdded { StockItemId = ItemId, Count = 2 });

        Because of =
            () => ApplyCommand(new RemoveItems { StockItemId = ItemId, Count = 5 });

        It should_not_have_removed_the_count_from_the_stock =
            () => NewEvents.OfType<ItemsRemoved>().ShouldNotContain(x => x.StockItemId == ItemId && x.Count == 5);
    }

    public class remove_all_items_from_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(new ItemsAdded { StockItemId = ItemId, Count = 2 });

        Because of =
            () => ApplyCommand(new RemoveItems { StockItemId = ItemId, Count = 2 });

        It should_not_have_removed_the_count_from_the_stock =
            () => NewEvents.OfType<ItemsRemoved>().ShouldContain(x => x.StockItemId == ItemId && x.Count == 2);
    }
}