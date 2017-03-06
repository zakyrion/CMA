using System;

namespace CMA.Messages
{
    public abstract class Message : Communication, IMessage
    {
        public virtual Type GetType()
        {
            return base.GetType();
        }
    }
}