using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scritchy.Infrastructure.Implementations;
using Scritchy.Domain;
using System.Reflection;

namespace Scritchy.Infrastructure.Configuration
{
    public class ConventionBasedRegistry : HandlerRegistry
    {
        public ConventionBasedRegistry()
            : this(AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public ConventionBasedRegistry(IEnumerable<Assembly> assemblies)
        {
            ScanAssembliesAndRegisterAll(assemblies);
        }

        void ScanAssembliesAndRegisterAll(IEnumerable<Assembly> assemblies)
        {
            var ARTypes = new List<Type>();
            var EventHandlers = new List<Type>();
            var PossibleCommandNames = new List<string>();
            var PossibleEventNames = new List<string>();
            var CommandTypes = new List<Type>();
            var EventTypes = new List<Type>();

            var srctypes = new List<Type>();

            // scan AR's & store possible command & event names
            foreach (var asm in assemblies)
            {
                srctypes.AddRange(asm.GetTypes()
                    .Where(x => !x.IsAbstract && x.IsClass && !x.IsGenericType && x.IsPublic && !x.Namespace.StartsWith("System") && !x.Namespace.StartsWith("Microsoft")));
            }
            foreach (var t in srctypes)
            {
                if (typeof(AR).IsAssignableFrom(t))
                    ARTypes.Add(t);
                foreach (var methodname in t.GetMethods().Where(x => x.ReturnType == typeof(void)).Select(x => x.Name))
                {
                    if (methodname.StartsWith("On") && methodname.Length > 2 && methodname[2] == methodname.ToUpper()[2])
                    {
                        PossibleEventNames.Add(methodname.Substring(2));
                        if (!EventHandlers.Contains(t))
                            EventHandlers.Add(t);
                    }
                    PossibleCommandNames.Add(methodname);
                }
            }
            foreach (var t in srctypes)
            {
                if (PossibleEventNames.Any(x => t.Name.StartsWith(x)))
                {
                    EventTypes.Add(t);
                }
                if (PossibleCommandNames.Any(x => t.Name.StartsWith(x)))
                {
                    CommandTypes.Add(t);
                }

            }

            RegisterHandlers(ARTypes, CommandTypes);
            RegisterHandlers(EventHandlers, EventTypes, "On");
        }

    }
}
