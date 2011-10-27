using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Scritchy.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<object> GetNewEventsSincePreviousRead();
        IEnumerable<object> EventsForInstance(object Instance);
        bool SaveEvents(IEnumerable<object> events);

    }

    public interface ICommandBus
    {
        void RunCommand(object Command);
    }

    public interface IEventApplier
    {
        void ApplyEventsToInstance(object instance,IEnumerable<object> events);
        void ApplyNewEventsToAllHandlers();
    }
    
    public interface IHandlerInstanceResolver
    {
        Scritchy.Domain.AR LoadARSnapshot(Type t, string Id);
        object ResolveHandlerFromType(Type t);
    }
}
