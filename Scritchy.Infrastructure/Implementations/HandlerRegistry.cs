using System;
using System.Collections.Generic;
using System.Linq;
using Scritchy.Infrastructure.Helpers;

namespace Scritchy.Infrastructure.Implementations
{
    public class HandlerRegistry
    {
        Dictionary<RegistrationKey, Action<object, object,IParameterResolver>> Handlers = new Dictionary<RegistrationKey, Action<object, object,IParameterResolver>>();

        public void Register(Type handlertype, Type messagetype, Action<object, object,IParameterResolver> handle)
        {
            var key = new RegistrationKey(handlertype, messagetype);
            if (Handlers.ContainsKey(key))
            {
                var oldhandle = Handlers[key];
                Handlers[key] = (instance, message,pr) =>
                {
                    oldhandle(instance, message,pr);
                    handle(instance, message,pr);
                };
            }
            else
            {
                Handlers.Add(key, handle);
            }
        }

        public void RegisterHandlers(IEnumerable<Type> InstanceTypes, IEnumerable<Type> MessageTypes,string methodnameprefix="")
        {
            foreach (var InstanceType in InstanceTypes)
            {
                var methodnames = InstanceType.GetMethods().Where(x => 
                    x.ReturnType == typeof(void) && 
                    x.Name.StartsWith(methodnameprefix) && 
                    (x.GetBaseDefinition()==null || x.GetBaseDefinition().DeclaringType != typeof(Scritchy.Domain.AR))
                    ).Select(x => x.Name).ToArray();
                foreach (var methodname in methodnames)
                {
                    var messagetype = MessageTypes.Where(x => x.Name == methodname.Substring(methodnameprefix.Length)).FirstOrDefault();
                    if (messagetype == null)
                        continue;
                    var localmn = methodname;
                    var localinstancetype = InstanceType;
                    var props = localinstancetype.GetMethod(localmn).GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
                    Action<object,object[]> invoke = (instance,parameters) => localinstancetype.GetMethod(localmn).Invoke(instance,parameters);
                    Action<object, object,IParameterResolver> invoker = (instance, message,pr) => {
                        var pars = pr.ResolveParameters(props, message).ToArray();
                        invoke(instance, pars);
                    };
                    this.Register(InstanceType, messagetype, invoker);
                }
            }
        }


        public IEnumerable<RegistrationKey> RegisteredHandlers
        {
            get
            {
                return Handlers.Keys;
            }
        }

        public Action<object, object,IParameterResolver> this[Type instanceType, Type eventtype]
        {
            get
            {
                return Handlers[new RegistrationKey(instanceType, eventtype)];
            }
        }

        public Action<object, object, IParameterResolver> this[RegistrationKey key]
        {
            get
            {
                return Handlers[key];
            }
        }

        public bool ContainsHandler(Type instanceType, Type eventtype)
        {
            return Handlers.ContainsKey(new RegistrationKey(instanceType, eventtype));
        }

        public class RegistrationKey
        {
            public RegistrationKey(Type InstanceType, Type MessageType)
            {
                this.InstanceType = InstanceType;
                this.MessageType = MessageType;
            }

            public Type InstanceType { get; protected set; }
            public Type MessageType { get; protected set; }

            public override bool Equals(object obj)
            {
                var cmp = obj as RegistrationKey;
                if (cmp == null)
                    return false;
                return InstanceType.Equals(cmp.InstanceType) && MessageType.Equals(cmp.MessageType);
            }

            public override int GetHashCode()
            {
                return InstanceType.GetHashCode() ^ MessageType.GetHashCode();
            }
        }
    }
}
