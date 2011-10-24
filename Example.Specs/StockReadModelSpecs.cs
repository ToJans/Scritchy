using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Example.Domain.Events;

namespace Example.Specs
{
    public class stock_read_model_handler:_in_stockcontext
    {
        Because of = () => {
            ApplyEvents(
                new ItemsAdded { StockItemId = "Item/1", Count = 5 },
                new ItemsAdded { StockItemId = "Item/2", Count = 7 },
                new ItemsRemoved { StockItemId = "Item/1", Count = 3 }
                );
        };

        It should_have_a_matching_amount_of_the_first_item = 
            ()=> Readmodel["Item/1"].ShouldEqual(2);

        It should_have_a_matching_amount_of_the_second_item =
            () => Readmodel["Item/2"].ShouldEqual(7);
    }
}
