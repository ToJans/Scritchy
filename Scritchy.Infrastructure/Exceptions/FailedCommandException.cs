using System;

namespace Scritchy.Infrastructure.Exceptions
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
