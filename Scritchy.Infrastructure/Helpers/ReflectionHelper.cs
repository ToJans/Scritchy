using System;
using System.Collections.Generic;
using System.Linq;

namespace Scritchy.Infrastructure.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> AllPublicTypesInNameSpace(this Type type)
        {
            return type.Assembly.GetTypes().Where(x => x.Namespace == type.Namespace && x.IsPublic == true);
        }


        public static Action<object, object> GetMessageInvokerAction(Type instanceType, Type messageType, string methodname)
        {
            var method = instanceType.GetMethod(methodname);
            if (method == null)
                return (object instance, object message) => { };
            var parametergetters = new List<Func<object,object>>();
            var msgprops = messageType.GetProperties().ToDictionary(x=>x.Name,x=>x);
            foreach (var methodparameter in method.GetParameters())
            {
                if (!msgprops.ContainsKey(methodparameter.Name))
                {
                    var s = string.Format("Method {2}.{3} has a parameter {4} {1} that is not defined in the type {0}"
                        , messageType.Name, methodparameter.Name, instanceType.Name, method.Name,methodparameter.ParameterType.Name);
                    throw new InvalidOperationException(s);
                }
                var messageproperty = msgprops[methodparameter.Name];
                if (!methodparameter.ParameterType.IsAssignableFrom(messageproperty.PropertyType))
                {
                    var s = string.Format("Parameter {1} in method {2}.{3} needs to have the same type as property {0}.{1}: {4} instead of {5}"
                        , messageType.Name, methodparameter.Name, instanceType.Name, method.Name
                        ,messageproperty.PropertyType.Name,methodparameter.ParameterType.Name);
                    throw new InvalidOperationException(s);
                }
                parametergetters.Add(msg => messageproperty.GetValue(msg, null));
            }
            return (object instance, object message) =>
            {
                var parameters = parametergetters.Select(x => x(message)).ToArray();
                method.Invoke(instance, parameters);
            };
        }
    }
}
