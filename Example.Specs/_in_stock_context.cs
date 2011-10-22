using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Example.Domain.Infrastructure;
using Machine.Specifications;
using Example.Domain.Implementation.Readmodel;

namespace Example.Specs
{
    public abstract class _in_stockcontext
    {
        static ExampleBus SUT;
        static int neweventindex = 0;
        protected static StockDictionary Readmodel;

        protected const string ItemId = "Item/1";

        Establish context = () =>
        {
            Readmodel = new StockDictionary();
            var handler = new StockDictionaryHandler(Readmodel);
            Func<Type, object> handlerLoader = t =>
            {
                if (t == typeof(StockDictionaryHandler))
                    return handler;
                else
                    throw new InvalidOperationException();
            };
            SUT = new ExampleBus(handlerLoader);
        };

        protected static void ApplyCommand(object command)
        {
            neweventindex = SUT.PublishedEvents.Count();
            SUT.RunCommand(command);
        }

        protected static IEnumerable<object> NewEvents
        {
            get
            {
                return SUT.PublishedEvents.Skip(neweventindex);
            }
        }

        protected static void ApplyEvents(params object[] events)
        {
            SUT.PublishedEvents.AddRange(events);
            SUT.ApplyEventsToHandlers(events);
        }
    }
}
