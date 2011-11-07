using System;
using System.Collections.Generic;

namespace Scritchy.Infrastructure.Exceptions
{
    public class SaveEventsException : Exception
    {
        public IEnumerable<object> Events;
    }
}
