//   Copyright {CMA} {Kharsun Sergei}
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
using CMA.Messages;

namespace CMA.Markers
{
    public class MessageMarkerHandler<T> : IMessageMarkerHandler where T : IMarker
    {
        protected MessageMarkerDelegate<IMessage, T> DelegateField;

        public MessageMarkerHandler(MessageMarkerDelegate<IMessage, T> @delegate)
        {
            DelegateField = @delegate;
        }

        public bool Equals(IMessageMarkerHandler handler)
        {
            return Contains(handler.Delegate);
        }

        public void Invoke(IMessage message)
        {
            message.LockMessage();
            if (!message.IsDone)
            {
                var marker = message.GetMarker<T>();
                DelegateField.Invoke(message, marker);
            }
            message.UnlockMessage();
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
            return @delegate == (Delegate)DelegateField;
        }
    }
}