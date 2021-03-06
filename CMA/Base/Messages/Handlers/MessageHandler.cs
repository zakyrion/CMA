﻿//   Copyright {CMA} {Kharsun Sergei}
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;

namespace CMA.Messages
{
    public class MessageHandler<T> : IMessageHandler where T : IMessage
    {
        protected MessageDelegate<T> DelegateField;

        public MessageHandler(MessageDelegate<T> @delegate)
        {
            DelegateField = @delegate;
        }

        public string Key
        {
            get { return typeof(T).ToString(); }
        }

        public Delegate Delegate
        {
            get { return DelegateField; }
        }

        public bool Contains(Delegate @delegate)
        {
            return @delegate == (Delegate) DelegateField;
        }

        public bool Equals(IMessageHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public void Invoke(IMessage message)
        {
            if (!message.IsDone)
            {
                message.LockMessage();
                DelegateField((T) message);
                //message.Done();
                message.UnlockMessage();
            }
        }
    }
}