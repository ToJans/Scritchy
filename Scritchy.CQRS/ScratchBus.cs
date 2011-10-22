using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Scritchy.CQRS
{
    public abstract class ScratchBus
    {
        class CommandInfo
        {
            public Func<object, IEnumerable<object>> Runner = null;
            public Func<object, string> GetId = null;
            public Type ARType = null;

            public static CommandInfo Create<AR,CMD>(Func<CMD, string> GetId, Func<CMD, IEnumerable<object>> Runner) 
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

        abstract protected object LoadInstanceFromTypeWithId(Type type,string Id);
        abstract protected void SaveEvents(IEnumerable<object> events);
        abstract protected object ResolveHandlerFromType(Type t);


        public void RunCommand(object command)
        {
            var name = command.GetType().Name;
            var events = RunnerForCommandTypeName[name].Runner(command);
            SaveEvents(events);
            ApplyEventsToHandlers(events);
        }

        public ScratchBus RegisterCommandHandlers(IEnumerable<Type> HandlerTypes, IEnumerable<Type> CommandTypes)
        {
            var ct = CommandTypes.ToDictionary(x=>x.Name,x=>x);
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

        ScratchBus RegisterCommand(Type ARType, Type CommandType)
        {
            var key = CommandType.Name;
            var idname = ARType.Name+"Id";
            Func<object,string> GetId = x => CommandType.GetProperty(idname).GetValue(x,null) as string;
            RunnerForCommandTypeName[key] = new CommandInfo()
            {
                ARType = ARType,
                GetId = GetId,
                Runner = cmd =>
                {
                    var AR = LoadInstanceFromTypeWithId(ARType, GetId(cmd));
                    AR.TryToExecuteSerializedMethodCall(cmd);
                    return (AR as ScratchAR) .Changes.GetPublishedEvents();
                }
            };
            return this;
        }

        public ScratchBus RegisterCommand<TAR, TCommand>(Func<TCommand, string> GetId) where TAR : ScratchAR, new()
        {
            var key = typeof(TCommand).Name;
            RunnerForCommandTypeName[key] = CommandInfo.Create<TAR,TCommand>(
                x => GetId(x),
                cmd =>
                {
                    var AR = (TAR)LoadInstanceFromTypeWithId(typeof(TAR), GetId(cmd));
                    AR.TryToExecuteSerializedMethodCall(cmd);
                    return AR.Changes.GetPublishedEvents();
                });
            return this;
        }

        Dictionary<Tuple<Type, Type>, Action<object, object>> ApplyEventToInstanceInfoForEventTypeAndInstanceType = new Dictionary<Tuple<Type, Type>, Action<object, object>>();

        public ScratchBus RegisterEventHandlers(IEnumerable<Type> Handlers, IEnumerable<Type> EventTypes)
        {
            foreach (var h in Handlers)
            {
                var p = h.GetMethods().Where(x => x.Name.StartsWith("On") && x.ReturnType == typeof(void));
                foreach (var et in EventTypes.Where(x => p.Any(y => y.Name == "On" + x.Name)))
                {
                    ApplyEventToInstanceInfoForEventTypeAndInstanceType[Tuple.Create(et, h)] = (e, i) =>
                    {
                        i.TryToExecuteSerializedMethodCall(e, methodPrefix: "On");
                    };
                }
            }
            return this;
        }

        protected IEnumerable<Type> EventTypesForHandler(Type HandlerType)
        {
            return ApplyEventToInstanceInfoForEventTypeAndInstanceType.Keys.Where(x => x.Item2 == HandlerType).Select(x => x.Item1).Distinct();
        }

        protected Predicate<object> EventFilterForARInstance(Type TAR,string ARId)
        {
            var types = EventTypesForHandler(TAR);
            return x => ApplyEventToInstanceInfoForEventTypeAndInstanceType.Any(y =>
                y.Key.Item1 == x.GetType() &&
                y.Key.Item2 == TAR && 
                (y.Key.Item1.GetProperty(TAR.Name+"Id").GetValue(x,null) as string)== ARId
                );
        }

        protected void ApplyEventsToInstance(object AR,IEnumerable<object> events) 
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
                foreach(var k in ApplyEventToInstanceInfoForEventTypeAndInstanceType.Keys.Where(x=>x.Item1 == e.GetType()))
                {
                    if (k.Item2.IsSubclassOf(typeof(ScratchAR))) // skip AR's
                        continue;
                    var handler = ResolveHandlerFromType(k.Item2);
                    handler.TryToExecuteSerializedMethodCall(e, "On");
                }

            }
        }


    }
}
