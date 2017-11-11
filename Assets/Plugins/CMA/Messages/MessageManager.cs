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
using CMA.Core;
using UnityEngine;

namespace CMA.Messages
{
    public class MessageManager : IMessageManager
    {
        protected readonly object Lock = new object();

        protected Dictionary<string, List<IMessageHandler>> MessageRecievers =
            new Dictionary<string, List<IMessageHandler>>();

        protected IThreadController ThreadController;

        public MessageManager()
        {
            ThreadController = new ThreadPoolController();
        }

        public MessageManager(IThreadController threadController)
        {
            ThreadController = threadController;
        }

        public virtual void Quit()
        {
            lock (Lock)
            {
                MessageRecievers.Clear();
            }
        }

        #region Messages

        public string TraceMarker { protected get; set; }
        public IMessage Message { get; protected set; }

        public virtual void Receive<T>(Action<IMessage> @delegate)
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (!MessageRecievers.ContainsKey(key))
                    MessageRecievers.Add(key, new List<IMessageHandler>());

                MessageRecievers[key].Add(new SimpleMessageHandler<T>(@delegate));
            }
        }

        public virtual void Receive<T>(Action<T, IMessage> @delegate)
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (!MessageRecievers.ContainsKey(key))
                    MessageRecievers.Add(key, new List<IMessageHandler>());

                MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
            }
        }

        public bool CanRespounce(IMessage message)
        {
            lock (Lock)
            {
                return MessageRecievers.ContainsKey(message.GetKey());
            }
        }

        public virtual void RemoveReceiver<T>(Action<T, IMessage> @delegate)
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (MessageRecievers.ContainsKey(key))
                    MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
            }
        }

        public virtual void Responce(IMessage message)
        {
            lock (Lock)
            {
                ThreadController.Invoke(HandleMessage, message);
            }
        }

        private void HandleMessage(IMessage message)
        {
            lock (Lock)
            {
                try
                {
                    Message = message;
                    message.AddTrace(TraceMarker);
                    var key = message.GetKey();
                    if (MessageRecievers.ContainsKey(key))
                        for (var i = 0; i < MessageRecievers[key].Count; i++)
                            MessageRecievers[key][i].Invoke(message);
                }
                catch (Exception exception)
                {
                    message.ShowTrace();
                    Debug.Log($"Exception:{exception} ");

                    message.Fail();
                }
            }
        }

        #endregion
    }
}