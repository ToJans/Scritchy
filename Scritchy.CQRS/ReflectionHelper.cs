using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.CQRS
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> AllPublicTypesInNameSpace(this Type type)
        {
            return type.Assembly.GetTypes().Where(x => x.Namespace == type.Namespace && x.IsPublic == true);
        }

        public static bool TryToExecuteSerializedMethodCall(this object state, object serializedMethodCall,string methodPrefix="")
        {
            var et = serializedMethodCall.GetType();
            var sm = state.GetType().GetMethod(methodPrefix + et.Name);
            if (sm == null|| sm.ReturnType != typeof(void)) return false;
            var pars = new List<object>();
            foreach (var p in sm.GetParameters())
            {
                var cp = et.GetProperties().Where(x => x.Name == p.Name).Select(x => new { t = x.PropertyType, val = x.GetValue(serializedMethodCall, null) })
                    .Union(et.GetFields().Where(x => x.Name == p.Name).Select(x => new { t = x.FieldType, val = x.GetValue(serializedMethodCall) }));
                if (cp.Count() == 0)
                {
                    pars.Add(p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null);
                }
                else
                {
                    pars.Add(Convert.ChangeType(cp.First().val, p.ParameterType));
                }
            }
            sm.Invoke(state, pars.ToArray());
            return true;
        }

    }
}
