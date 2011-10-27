using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.Infrastructure.Exceptions
{
    public class SaveEventsException : Exception
    {
        public IEnumerable<object> Events;
    }
}
