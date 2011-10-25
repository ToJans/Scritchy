using Example.Domain.Events;
using Machine.Specifications;

namespace Example.Specs
{
    public class stock_read_model_handler:_in_stockcontext
    {
        Because of = () => {
            ApplyEvents(
                new ItemAllowed { StockItemId = "Item/1", Name = "Item 1" },
                new ItemAllowed { StockItemId = "Item/2", Name = "Item 2" },
                new ItemAllowed { StockItemId = "Item/3", Name = "Item 3" },
                new ItemsAdded { StockItemId = "Item/1", Amount = 5 },
                new ItemsAdded { StockItemId = "Item/2", Amount = 7 },
                new ItemsRemoved { StockItemId = "Item/1", Amount = 3 },
                new ItemBanned { StockItemId = "Item/3" }
                );
        };

        It should_have_2_item_types =
            () => Readmodel.Count.ShouldEqual(2);

        It should_have_a_matching_amount_of_the_first_item = 
            ()=> Readmodel["Item/1"].Amount.ShouldEqual(2);

        It should_have_a_matching_amount_of_the_second_item =
            () => Readmodel["Item/2"].Amount.ShouldEqual(7);
    }
}
