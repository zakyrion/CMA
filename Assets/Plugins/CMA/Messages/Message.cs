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
using System.Collections.Generic;

namespace CMA.Messages
{
    public class Message : Communication, IMessage
    {
        private Type _type;

        public Message(object data, IRespounceCode respounceCode = null)
        {
            Data = data;
            RespounceCode = respounceCode;
        }

        public object Data { get; protected set; }

        public IRespounceCode RespounceCode { get; protected set; }

        public List<string> Conditions { get; protected set; }

        public T GetData<T>()
        {
            return (T) Data;
        }

        public string Key => KeyType.ToString();
        public Type KeyType => _type ?? (_type = GetKeyType());

        public void ShowTrace()
        {
        }

        public void AddCondition(string condition)
        {
            if (Conditions == null)
                Conditions = new List<string>();

            Conditions.Add(condition);
        }

        protected virtual Type GetKeyType()
        {
            return Data.GetType();
        }
    }
}