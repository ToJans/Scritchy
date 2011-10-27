using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.CQRS
{
    public class SaveEventsException : Exception
    {
        public IEnumerable<object> Events;
    }
}
