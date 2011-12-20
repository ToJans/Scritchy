using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.Domain
{
    // inspired by @yreynhout:
    // url: http://seabites.wordpress.com/2010/10/31/guards-and-queries-in-the-domain-model/
    public static class Guard
    {
        public static void Against(bool assertion, string message = null)
        {
            if (assertion) throw new InvalidOperationException(message);
        }
    }

}
