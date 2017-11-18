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

using UnityEngine;

namespace CMA.Messages
{
    public class Message : Communication, IMessage
    {
        protected object Lock = new object();
        public Message(object data, IRespounceCode respounceCode = null)
        {
            Data = data;
            RespounceCode = respounceCode;

            if (Data != null)
                Key = Data.GetType().ToString();
        }

        public object Data { get; protected set; }

        public IRespounceCode RespounceCode { get; protected set; }

        public T GetData<T>()
        {
            return (T) Data;
        }

        public string Key { get; protected set; }

        public void ShowTrace()
        {
            Debug.Log("Message: " + Key);
            Debug.Log("Adress : " + Adress.AdressFull);
            foreach (var trace in Traces)
                Debug.Log("Trace: " + trace);
        }
    }
}