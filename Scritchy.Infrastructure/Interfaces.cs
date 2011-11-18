using System;
using System.Collections.Generic;

namespace Scritchy.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<object> GetNewEvents(object Instance=null,object EnumeratorContext=null);
        bool SaveEvents(IEnumerable<object> events);
    }

    public interface ICommandBus
    {
        void RunCommand(object Command);
    }

    public interface IEventApplier
    {
        void ApplyEventsToInstance(object instance);
        void ApplyNewEventsToAllHandlers();
    }
    
    public interface IHandlerInstanceResolver
    {
        Scritchy.Domain.AR LoadARSnapshot(Type t, string Id,IParameterResolver pr);
        object ResolveHandlerFromType(Type t);
    }

    public interface IParameterResolver
    {
        IEnumerable<object> ResolveParameters(IEnumerable<KeyValuePair<string, Type>> ParametersToResolve, object message);
    }


}
