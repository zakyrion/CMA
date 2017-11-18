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
using System.Collections.Generic;
using CMA.Core;
using UnityEngine;

namespace CMA.Messages
{
    public class MessageManager : IMessageManager
    {
        protected bool IsQuit;

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
            IsQuit = true;
        }

        #region Messages

        public virtual void Receive<T>(Action<IMessage> @delegate)
        {
            var key = typeof(T).ToString();

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(new SimpleMessageHandler<T>(@delegate));
        }

        public virtual void Receive<T>(Action<T, IMessage> @delegate)
        {
            var key = typeof(T).ToString();

            if (!MessageRecievers.ContainsKey(key))
                MessageRecievers.Add(key, new List<IMessageHandler>());

            MessageRecievers[key].Add(new MessageHandler<T>(@delegate));
        }

        public bool CanRespounce(IMessage message)
        {
            return MessageRecievers.ContainsKey(message.Key);
        }

        public virtual void RemoveReceiver<T>(Action<T, IMessage> @delegate)
        {
            var key = typeof(T).ToString();

            if (MessageRecievers.ContainsKey(key))
                MessageRecievers[key].Remove(MessageRecievers[key].Find(handler => handler.Contains(@delegate)));
        }

        public virtual void Responce(IMessage message)
        {
            ThreadController.Invoke(HandleMessage, message);
        }

        private void HandleMessage(IMessage message)
        {
            try
            {
                var key = message.Key;
                if (MessageRecievers.ContainsKey(key))
                    for (var i = 0; i < MessageRecievers[key].Count; i++)
                        if (!IsQuit)
                            MessageRecievers[key][i].Invoke(message);
            }
            catch (Exception exception)
            {
                message.ShowTrace();
                Debug.Log($"Exception:{exception} ");

                message.Fail();
            }
        }

        #endregion
    }
}