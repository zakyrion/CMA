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
using System.Linq;
using System.Threading;
using CMA.Markers;
using UnityEngine;

namespace CMA.Messages
{
    public class MessageManager : IMessageManager
    {
        private static int _nextId = int.MinValue;
        protected readonly object Lock = new object();

        protected Dictionary<string, List<IMessageMarkerHandler>> MessageMarker =
            new Dictionary<string, List<IMessageMarkerHandler>>();

        protected Dictionary<string, List<IMessageHandler>> MessageRecievers =
            new Dictionary<string, List<IMessageHandler>>();

        public MessageManager()
        {
            Id = NextId;
        }

        public static int NextId
        {
            get { return Interlocked.Increment(ref _nextId); }
        }

        #region Messages

        public int Id { get; protected set; }
        public string TraceMarker { protected get; set; }

        public void AddMarker(IMessageMarkerHandler handler)
        {
            if (!MessageMarker.ContainsKey(handler.Key))
                MessageMarker.Add(handler.Key, new List<IMessageMarkerHandler> { handler });
            else
                MessageMarker[handler.Key].Add(handler);
        }

        public void RemoveMarker(IMessageMarkerHandler handler)
        {
            if (MessageMarker.ContainsKey(handler.Key))
                MessageMarker[handler.Key].Remove(handler);
        }

        public virtual IMessageManager NewWithType()
        {
            return new MessageManager();
        }

        public virtual void SendMessage(IMessage message)
        {
            lock (Lock)
            {
                try
                {
                    var isDone = false;
                    message.AddTrace(TraceMarker);
                    var key = message.GetKey();

                    if (!message.IsAllMarkersCheck())
                    {
                        foreach (var marker in message.Markers)
                            if (MessageMarker.ContainsKey(marker.MarkerKey))
                            {
                                foreach (var handler in MessageMarker[marker.MarkerKey])
                                    handler.Invoke(message);
                                return;
                            }
                    }

                    if (MessageRecievers.ContainsKey(key))
                    {
                        for (var i = 0; i < MessageRecievers[key].Count; i++)
                            MessageRecievers[key][i].Invoke(message);

                        isDone = true;
                    }

                    if (!isDone)
                        message.Fail();
                }
                catch (Exception exception)
                {
                    message.ShowTrace();
                    Debug.Log(string.Format("Exception:{0} ", exception));

                    message.Fail();
                }
            }
        }

        public bool ContainsMessage<T>() where T : IMessage
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();
                return MessageRecievers.ContainsKey(key) || MessageMarker.ContainsKey(key);
            }
        }

        public bool ContainsMessage(IMessage message)
        {
            lock (Lock)
            {
                var key = message.GetKey();
                return (MessageRecievers.ContainsKey(key) && message.IsAllMarkersCheck()) ||
                       message.Markers.Any(marker => MessageMarker.ContainsKey(marker.MarkerKey)) ||
                       MessageMarker.ContainsKey(key);
            }
        }

        public virtual void SubscribeMessage<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (!MessageRecievers.ContainsKey(key))
                    MessageRecievers.Add(key, new List<IMessageHandler>());

                MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
            }
        }

        public void SubscribeMessage<T>(MessageDelegate<IMessage> @delegate)
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (!MessageRecievers.ContainsKey(key))
                    MessageRecievers.Add(key, new List<IMessageHandler>());

                MessageRecievers[key].Add(new MessageHandler<IMessage>(@delegate));
            }
        }

        public virtual void RemoveMessageReciever<T>(MessageDelegate<T> @delegate) where T : IMessage
        {
            lock (Lock)
            {
                var key = typeof(T).ToString();

                if (MessageRecievers.ContainsKey(key))
                    MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
            }
        }

        #endregion

        #region Requests

        public virtual void InvokeAtManager(Action action)
        {
            lock (Lock)
            {
                action();
            }
        }

        public virtual void Quit()
        {
            lock (Lock)
            {
                MessageMarker.Clear();
                MessageRecievers.Clear();
            }
        }

        #endregion
    }
}