using Machine.Specifications;
using Commands = Example.Domain.Commands;
using Events = Example.Domain.Events;

namespace Example.Specs
{
    public class allow_item_to_stock : _in_stockcontext
    {
        Because of =
            () => ApplyCommand(new Commands.AllowItem { StockItemId = ItemId, Name = "Item 1" });

        It should_have_allowed_the_item_to_the_stock =
            () => ResultingEvents<Events.ItemAllowed>().ShouldContain(x => x.StockItemId == ItemId && x.Name == "Item 1");
    }

    public class ban_an_item_from_Stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(new Events.ItemAllowed { StockItemId = ItemId });

        Because of =
            () => ApplyCommand(new Commands.BanItem { StockItemId = ItemId });

        It should_have_banned_the_item_from_the_stock =
            () => ResultingEvents<Events.ItemBanned>().ShouldContain(x => x.StockItemId == ItemId);
    }

    public class ban_an_item_from_Stock_that_is_still_in_stock : _in_stockcontext
    {
        Establish context =
            () => ApplyEvents(
                new Events.ItemAllowed { StockItemId = ItemId },
                new Events.ItemsAdded { StockItemId = ItemId, Amount = 2 }
                );

        Because of =
            () => Try(() => ApplyCommand(new Commands.BanItem { StockItemId = ItemId }));

        It should_fail =
            () => Exception.ShouldNotBeNull();
    }

}