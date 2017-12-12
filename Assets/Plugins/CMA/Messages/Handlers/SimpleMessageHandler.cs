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
    public class SimpleMessageHandler<T> : IMessageHandler
    {
        protected Action<IMessage> DelegateField;

        public SimpleMessageHandler(Action<IMessage> @delegate)
        {
            DelegateField = @delegate;
        }

        public string Key => typeof(T).ToString();

        public Delegate Delegate => DelegateField;

        public bool Contains(Delegate @delegate)
        {
            return @delegate == (Delegate) DelegateField;
        }

        public bool Equals(IMessageHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public bool IsParent(object obj)
        {
            return DelegateField.Target == obj;
        }

        public void Invoke(IMessage message)
        {
            DelegateField(message);
        }
    }
}