using System;
using System.Collections.Generic;
using System.Linq;
using Example.Domain.Readmodel;
using Example.Infrastructure;
using Machine.Specifications;
using Scritchy.Infrastructure;
using Scritchy.Infrastructure.Implementations;

namespace Example.Specs
{
    public abstract class _in_stockcontext
    {
        static ICommandBus SUT;
        static IEventStore eventstore;
        static IEventApplier eventapplier;

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
            var registry = new ExampleRegistry();
            eventstore = new InMemoryEventStore(registry);
            var resolver = new HandlerInstanceResolver(eventstore,registry,handlerLoader);
            eventapplier = new EventApplier(eventstore, registry, resolver);
            SUT = new CommandBus(eventstore, registry, resolver);
        };

        protected static void ApplyCommand(object command)
        {
            SUT.RunCommand(command);
            PublishedEvents.Clear();
            PublishedEvents.AddRange(eventstore.GetNewEvents());
        }

        protected static IEnumerable<T> ResultingEvents<T>(Predicate<T> pred=null)
        {
            if (pred == null) pred = x=>true;
            return PublishedEvents.OfType<T>().Where(x=>pred(x));
        }

        protected static void ApplyEvents(params object[] events)
        {
            eventstore.SaveEvents(events);
            eventapplier.ApplyNewEventsToAllHandlers();
        }

        protected static void Try(Action a)
        {
            Exception = Catch.Exception(a);
        }

    }
}
