using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scritchy.Domain;

namespace Scritchy.Infrastructure.Implementations
{
    public class ParameterResolver: IParameterResolver 
    {
        IHandlerInstanceResolver handlerresolver;

        public ParameterResolver(IHandlerInstanceResolver handlerresolver)
        {
            this.handlerresolver = handlerresolver;
        }

        public IEnumerable<object> ResolveParameters(IEnumerable<KeyValuePair<string, Type>> ParametersToResolve, object message)
        {
            var props = message.GetType().GetProperties().ToDictionary(x => x.Name);
            foreach (var p in ParametersToResolve)
            {
                if (typeof(AR).IsAssignableFrom(p.Value))
                {
                    var propname = p.Key+"Id";
                    if (!props.ContainsKey(propname) || props[propname].PropertyType!=typeof(string))
                    {
                        var s = string.Format("Missing a public property \"string {0}\" in the message of type {1}",propname,message.GetType().Name);
                        throw new InvalidOperationException(s);
                    }
                    var key = props[propname].GetValue(message,null) as string;
                    if (key == null)
                        yield return null;
                    var ARInstance = handlerresolver.LoadARSnapshot(p.Value,key,this);
                    yield return ARInstance;
                }
                else if (props.ContainsKey(p.Key))
                {
                    var val = props[p.Key].GetValue(message, null);
                    if (val != null && p.Value != props[p.Key].PropertyType)
                        val = Convert.ChangeType(val, p.Value);
                    yield return val;
                }
                else
                {
                    var service = handlerresolver.ResolveHandlerFromType(p.Value);
                    yield return service;
                }
            }
        }
    }
}
