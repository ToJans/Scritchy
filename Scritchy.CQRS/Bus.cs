using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Scritchy.CQRS
{
    public abstract class Bus
    {
        class CommandInfo
        {
            public Func<object, IEnumerable<object>> Runner = null;
            public Func<object, string> GetId = null;
            public Type ARType = null;

            public static CommandInfo Create<AR, CMD>(Func<CMD, string> GetId, Func<CMD, IEnumerable<object>> Runner)
            {
                var ci = new CommandInfo()
                {
                    ARType = typeof(AR),
                    GetId = cmd => GetId((CMD)cmd),
                    Runner = ar => Runner((CMD)ar)
                };
                return ci;
            }

            public CommandInfo() { }

        }

        Dictionary<string, CommandInfo> RunnerForCommandTypeName = new Dictionary<string, CommandInfo>();

        abstract protected object LoadAR(Type type, string Id);
        abstract protected void SaveEvents(IEnumerable<object> events);
        abstract protected object ResolveHandlerFromType(Type t);


        public void RunCommand(object command)
        {
            var name = command.GetType().Name;
            var runner = RunnerForCommandTypeName[name].Runner;
            IEnumerable<object> events = new object[] { };
            try
            {
                events = runner(command);
            }
            catch (TargetInvocationException e)
            {
                throw new FailedCommandException(e.InnerException,command);
            }
            SaveEvents(events);
        }

        public Bus RegisterCommandHandlers(IEnumerable<Type> HandlerTypes, IEnumerable<Type> CommandTypes)
        {
            var ct = CommandTypes.ToDictionary(x => x.Name, x => x);
            dynamic self = this;
            foreach (var ht in HandlerTypes)
            {
                foreach (var m in ht.GetMethods().Where(x => x.ReturnType == typeof(void)))
                {
                    if (ct.ContainsKey(m.Name))
                    {
                        RegisterCommand(ht, ct[m.Name]);
                        ct.Remove(m.Name);
                    }
                }
            }
            return this;
        }

        Bus RegisterCommand(Type ARType, Type CommandType)
        {
            var key = CommandType.Name;
            var idname = ARType.Name + "Id";
            Func<object, string> GetId = x => CommandType.GetProperty(idname).GetValue(x, null) as string;
            RunnerForCommandTypeName[key] = new CommandInfo()
            {
                ARType = ARType,
                GetId = GetId,
                Runner = cmd =>
                {
                    var AR = LoadAR(ARType, GetId(cmd));
                    AR.TryToExecuteSerializedMethodCall(cmd);
                    return (AR as AR).Changes.GetPublishedEvents();
                }
            };
            return this;
        }

        public Bus RegisterCommand<TAR, TCommand>(Func<TCommand, string> GetId) where TAR : AR, new()
        {
            var key = typeof(TCommand).Name;
            RunnerForCommandTypeName[key] = CommandInfo.Create<TAR, TCommand>(
                x => GetId(x),
                cmd =>
                {
                    var AR = (TAR)LoadAR(typeof(TAR), GetId(cmd));
                    AR.TryToExecuteSerializedMethodCall(cmd);
                    return AR.Changes.GetPublishedEvents();
                });
            return this;
        }

        protected class EventHandlerReference
        {
            public Type EventType;
            public Type HandlerType;
            public Action<object, object> Handle;
        }

        List<EventHandlerReference> EventHandlers = new List<EventHandlerReference>();

        public Bus RegisterEventHandlers(IEnumerable<Type> Handlers, IEnumerable<Type> EventTypes)
        {
            foreach (var h in Handlers)
            {
                var p = h.GetMethods().Where(x => x.Name.StartsWith("On") && x.ReturnType == typeof(void));
                foreach (var et in EventTypes.Where(x => p.Any(y => y.Name == "On" + x.Name)))
                {
                    EventHandlers.Add(new EventHandlerReference
                    {
                        EventType = et,
                        HandlerType = h,
                        Handle = (e, i) => i.TryToExecuteSerializedMethodCall(e, methodPrefix: "On")
                    });
                }
            }
            return this;
        }

        protected IEnumerable<Type> EventTypesForHandler(Type HandlerType)
        {
            return EventHandlers.Where(x => x.HandlerType == HandlerType).Select(x => x.EventType).Distinct();
        }

        protected Predicate<object> EventFilterForARInstance(Type TAR, string ARId)
        {
            var types = EventTypesForHandler(TAR);
            return x => EventHandlers.Any(y =>
                y.EventType == x.GetType() &&
                y.HandlerType == TAR &&
                (y.EventType.GetProperty(TAR.Name + "Id").GetValue(x, null) as string) == ARId
                );
        }

        protected void ApplyEventsToInstance(object AR, IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                AR.TryToExecuteSerializedMethodCall(e, "On");
            }
        }

        public void ApplyEventsToHandlers(IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                foreach (var k in EventHandlers.Where(x => x.EventType == e.GetType()))
                {
                    if (k.HandlerType.IsSubclassOf(typeof(AR))) // skip AR's
                        continue;
                    var handler = ResolveHandlerFromType(k.HandlerType);
                    handler.TryToExecuteSerializedMethodCall(e, "On");
                }

            }
        }


    }
}
