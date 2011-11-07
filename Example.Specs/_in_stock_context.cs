using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Readmodel;
using Machine.Specifications;
using Scritchy.Infrastructure;

namespace Example.Specs
{
    public abstract class _in_stockcontext
    {
        static ScritchyBus SUT;

        protected static StockDictionary Readmodel;
        protected static Exception Exception;
        static List<object> PublishedEvents=new List<object>();

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
            SUT = new ScritchyBus(handlerLoader);
        };

        protected static void ApplyCommand(object command)
        {
            SUT.RunCommand(command);
            PublishedEvents.AddRange(SUT.EventStore.GetNewEvents());
        }

        protected static IEnumerable<T> ResultingEvents<T>(Predicate<T> pred=null)
        {
            if (pred == null) pred = x=>true;
            return PublishedEvents.OfType<T>().Where(x=>pred(x));
        }

        protected static void ApplyEvents(params object[] events)
        {
            SUT.EventStore.SaveEvents(events);
            SUT.ApplyNewEventsToAllHandlers();
            // make sure GetNewEvents only returns events launched after all these events
            PublishedEvents.AddRange(SUT.EventStore.GetNewEvents());
            PublishedEvents.Clear();
        }

        protected static void Try(Action a)
        {
            Exception = Catch.Exception(a);
        }

    }
}
