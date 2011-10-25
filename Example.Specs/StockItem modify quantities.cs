using Machine.Specifications;
using Commands = Example.Domain.Commands;
using Events = Example.Domain.Events;

namespace Example.Specs
{
    public class add_allowed_items_to_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(new Events.ItemAllowed {StockItemId = ItemId} );

        Because of =
            () => ApplyCommand(new Commands.AddItems { StockItemId = ItemId, Amount = 5 });

        It should_have_added_the_count_to_the_stock =
            () => ResultingEvents<Events.ItemsAdded>().ShouldContain(x => x.StockItemId == ItemId && x.Amount == 5);
    }

    public class add_non_allowed_items_to_stock : _in_stockcontext
    {

        Because of =
            () => Try(()=>ApplyCommand(new Commands.AddItems { StockItemId = ItemId, Amount = 5 }));

        It should_fail =
            () => Exception.ShouldNotBeNull();
    }

    
    public class remove_a_few_items_from_the_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(
                new Events.ItemAllowed {StockItemId = ItemId},
                new Events.ItemsAdded { StockItemId = ItemId, Amount = 10 }
                );

        Because of =
            () => ApplyCommand(new Commands.RemoveItems { StockItemId = ItemId, Amount = 5 });

        It should_have_removed_the_count_from_the_stock =
            () => ResultingEvents<Events.ItemsRemoved>().ShouldContain(x => x.StockItemId == ItemId && x.Amount == 5);
    }

    public class remove_to_much_from_stock : _in_stockcontext
    {
        Establish context = 
            () => ApplyEvents(
                new Events.ItemAllowed { StockItemId = ItemId },
                new Events.ItemsAdded { StockItemId = ItemId, Amount = 2 }
                );

        Because of =
            () => Try(()=>ApplyCommand(new Commands.RemoveItems { StockItemId = ItemId, Amount = 5 }));

        It should_fail =
            () => Exception.ShouldNotBeNull();
    }

    public class remove_all_items_from_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(
                new Events.ItemAllowed { StockItemId = ItemId },
                new Events.ItemsAdded { StockItemId = ItemId, Amount = 2 }
                );

        Because of =
            () => ApplyCommand(new Commands.RemoveItems { StockItemId = ItemId, Amount = 2 });

        It should_have_removed_the_count_from_the_stock =
            () => ResultingEvents<Events.ItemsRemoved>().ShouldContain(x => x.StockItemId == ItemId && x.Amount == 2);
    }
}