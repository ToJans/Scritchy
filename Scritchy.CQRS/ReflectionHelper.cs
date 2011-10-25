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
                var cp = et.GetProperties()
                    .Where(x => x.Name == p.Name)
                    .Select(x => new { t = x.PropertyType, val = x.GetValue(serializedMethodCall, null) });
                if (cp.Count() == 0)
                {
                    var s = string.Format("Missing parameter {0}.{1} for method {2}.{3}",et.Name,p.Name,state.GetType().Name,sm.Name);
                    throw new InvalidOperationException(s);
                    //pars.Add(p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null);
                }
                else
                {
                    try
                    {
                        pars.Add(Convert.ChangeType(cp.First().val, p.ParameterType));
                    }
                    catch (Exception)
                    {
                        var s = string.Format("parameter {0}.{1} can not be converted to from {4} to {5} for method {2}.{3}", et.Name, p.Name, state.GetType().Name, sm.Name,cp.First().t.Name, p.ParameterType.Name);
                        throw new InvalidOperationException(s);
                    }
                }
            }
            sm.Invoke(state, pars.ToArray());
            return true;
        }

    }
}
