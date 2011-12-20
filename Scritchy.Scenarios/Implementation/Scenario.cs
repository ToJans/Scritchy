using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scritchy.Scenarios.Implementation
{
    public abstract class Scenario
    {
        List<object> GivenEvents = new List<object>();
        object WhenCommand = null;
        Dictionary<string,Predicate<IEnumerable<object>>> ThenEvents = new Dictionary<string,Predicate<IEnumerable<object>>>();
        Predicate<object> ExceptionAction=null;
        

        public class TestResult
        {
            public string Description;
            public bool Assertion;
        }

        protected void Given(object @event)
        {
            GivenEvents.Add(@event);
        }

        protected void When(object Command)
        {
            WhenCommand = Command;
        }

        protected void ThenException<T>(Predicate<T> IsExpectedException) where T:class 
        {
            ExceptionAction = x => (x is T) && IsExpectedException(x as T);
        }

        protected Predicate<IEnumerable<object>> this[string description]
        {
            set
            {
                ThenEvents.Add(description, value);
            }
        }

        public string[] Run()
        {
            var b = new Scritchy.Infrastructure.ScritchyBus();
            b.EventStore.SaveEvents(GivenEvents);
            List<object> events = new List<object>(b.EventStore.GetNewEvents());
            if (WhenCommand == null)
                throw new InvalidOperationException("You have to execute a command");
            events.Clear();
            events.AddRange(b.EventStore.GetNewEvents());
            return ThenEvents.Where(x=>x.Value(events)).Select(x=>x.Key).ToArray();
        }
    }
}