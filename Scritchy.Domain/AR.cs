using System;

namespace Scritchy.Domain
{
    public abstract class AR
    {
        public string Id { get; set; }
        public Events Changes;
        public Action<object> TryApplyEvent
        {
            set {
                Changes = new Events(this,value);
            }
        }

        public AR()
        {
            
        }

    }

}
