using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.CQRS
{
    public class FailedCommandException:Exception
    {
        public FailedCommandException(Exception e, object command)
            : base(e.Message, e.InnerException)
        {
            Command = command;
        }

        public object Command {get;set;}
    }
}
