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


    }
}
