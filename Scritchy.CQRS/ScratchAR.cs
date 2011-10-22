using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scritchy.CQRS
{
    public abstract class ScratchAR
    {
        public string Id { get; set; }
        public ScratchEvents Changes; 

        public ScratchAR()
        {
            Changes = new ScratchEvents(this);
        }
    }

}
