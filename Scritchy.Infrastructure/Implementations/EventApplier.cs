using System.Linq;
using System.Reflection;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{
    public class EventApplier : IEventApplier
    {
        IEventStore eventstore;
        HandlerRegistry handlerregistry;
        IHandlerInstanceResolver resolver;
        IParameterResolver parameterresolver;
        static readonly Helpers.Synchronizer<object> mySync = new Helpers.Synchronizer<object>();

        public EventApplier(IEventStore eventstore, HandlerRegistry handlerregistry, IHandlerInstanceResolver resolver,IParameterResolver parameterresolver)
        {
            this.eventstore = eventstore;
            this.handlerregistry = handlerregistry;
            this.resolver = resolver;
            this.parameterresolver = parameterresolver;
        }

        public void ApplyEventsToInstance(object instance)
        {
            var instancetype = instance.GetType();
            var enumeratorcontext = instance is AR ? instance : instancetype;
            object lockKey = instancetype;
            if (instance is AR)
                lockKey = (instance as AR).Id;
            foreach (var evt in eventstore.GetNewEvents(instance,enumeratorcontext))
            {
                if (!this.handlerregistry.ContainsHandler(instancetype,evt.GetType()))
                    continue;
                var handler = this.handlerregistry[instancetype, evt.GetType()];
                try
                {
                    using (var mylock = mySync.Lock(lockKey))
                    {
                        lock (lockKey)
                        {
                            handler(instance, evt,parameterresolver);
                        }
                    }
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
        }

        public void ApplyNewEventsToAllHandlers()
        {
            foreach(var handlertype in handlerregistry.RegisteredHandlers
                .Where(x=>!typeof(AR).IsAssignableFrom(x.InstanceType))
                .Select(x=>x.InstanceType).Distinct())
            {
                object handler=resolver.ResolveHandlerFromType(handlertype);
                ApplyEventsToInstance(handler);
            }
        }
    }
}
