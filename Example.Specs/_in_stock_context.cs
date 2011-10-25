using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Readmodel;
using Example.Infrastructure;
using Machine.Specifications;

namespace Example.Specs
{
    public abstract class _in_stockcontext
    {
        static ExampleBus SUT;
        static int neweventindex = 0;
        protected static StockDictionary Readmodel;
        protected static Exception Exception;

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

        protected static IEnumerable<T> ResultingEvents<T>(Predicate<T> pred=null)
        {
            if (pred == null) pred = x=>true;
            return SUT.PublishedEvents.Skip(neweventindex).OfType<T>().Where(x=>pred(x));
        }

        protected static void ApplyEvents(params object[] events)
        {
            SUT.PublishedEvents.AddRange(events);
            SUT.ApplyEventsToHandlers(events);
        }

        protected static void Try(Action a)
        {
            Exception = Catch.Exception(a);
        }

    }
}
