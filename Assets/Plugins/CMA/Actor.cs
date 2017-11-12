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
using CMA.Messages;
using UnityEngine;

namespace CMA
{
    public class Actor : IActor
    {
        private readonly List<IMessage> _forSend = new List<IMessage>();
        private int _id;
        protected Dictionary<IRespounceCode, IActionHandler> Actions = new Dictionary<IRespounceCode, IActionHandler>();

        protected Func<IMessage[]> MessagesRequest;

        public Actor()
        {
            ThreadController = new ThreadPoolController();
            Manager = new MessageManager(ThreadController);

            Receive<CallBack>(OnCallback);
            Receive<Kill>(OnKill);

            Subscribe();
        }

        public Actor(IThreadController threadController)
        {
            ThreadController = threadController;
            Manager = new MessageManager(ThreadController);

            Receive<CallBack>(OnCallback);
            Receive<Kill>(OnKill);

            Subscribe();
        }

        protected int RespounceId => _id++;

        public IMessageManager Manager { get; protected set; }

        public IThreadController ThreadController { get; protected set; }
        public string Adress => MailBox?.Adress.AdressFull;
        public IMailBox MailBox { get; protected set; }

        public void CheckMailBox()
        {
            ThreadController.Invoke(CheckMailBoxHandler);
        }

        public void OnAdd(IMailBox mailBox, Func<IMessage[]> messagesRequest)
        {
            MessagesRequest = messagesRequest;
            MailBox = mailBox;

            CheckMailBox();

            foreach (var message in _forSend)
            {
                if (string.IsNullOrEmpty(mailBox.Adress.AdressFull))
                    Debug.Log($"{GetType()}");
                message.SetBackAdress(MailBox.Adress);
                MailBox.SendMail(message);
            }

            _forSend.Clear();
        }

        public virtual void Quit()
        {
            ThreadController.Remove();
            Manager.Quit();
        }

        public virtual void Send(object data, string adress = "")
        {
            var message = new Message(data);

            if (MailBox != null)
            {
                message.Init(new Adress(adress), MailBox.Adress);
                MailBox.SendMail(message);
            }
            else
            {
                message.SetAdress(new Adress(adress));
                _forSend.Add(message);
            }
        }

        public virtual void Send(object data, Action action, string adress = "")
        {
            Message message;
            if (action != null)
            {
                IActionHandler handler = new ActionHandler(action);
                IRespounceCode code = new RespounceCode(RespounceId);

                Actions.Add(code, handler);
                message = new Message(data, code);
            }
            else
            {
                message = new Message(data);
            }

            if (MailBox != null)
            {
                message.Init(new Adress(adress), MailBox.Adress);
                MailBox.SendMail(message);
            }
            else
            {
                message.SetAdress(new Adress(adress));
                _forSend.Add(message);
            }
        }

        public void Ask<TR>(Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new SimpleRequest<TR>(code);

            if (MailBox != null)
            {
                request.Init(new Adress(adress), MailBox.Adress);
                MailBox.SendMail(request);
            }
            else
            {
                request.SetAdress(new Adress(adress));
                _forSend.Add(request);
            }
        }

        public virtual void Ask<TM, TR>(TM data, Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new Request<TR>(data, code);

            if (MailBox != null)
            {
                request.Init(new Adress(adress), MailBox.Adress);
                MailBox.SendMail(request);
            }
            else
            {
                request.SetAdress(new Adress(adress));
                _forSend.Add(request);
            }
        }

        public void Respounce(IMessage message, object data = null)
        {
            Debug.Log($"At: {Adress} Respounce: {message.GetKey()} by Adress: {message.BackAdress}");

            var callback = new Message(new CallBack(message.RespounceCode, data));

            if (MailBox != null)
            {
                callback.Init(message.BackAdress, MailBox.Adress);
                MailBox.SendMail(callback);
            }
            else
            {
                callback.SetAdress(message.BackAdress);
                _forSend.Add(callback);
            }
        }

        private void OnKill(IMessage obj)
        {
            ThreadController.Invoke(OnKillHandler);
        }

        private void OnKillHandler()
        {
            Quit();
        }

        private void CheckMailBoxHandler()
        {
            if (MessagesRequest != null)
            {
                var result = MessagesRequest();
                foreach (var message in result)
                    Manager.Responce(message);
            }
        }

        protected virtual void Subscribe()
        {
        }

        internal virtual void OnCallback(CallBack callBack, IMessage message)
        {
            Debug.Log(
                $"At: {Adress} To: {message.Adress} Catch Callback: Hash {callBack.Param1.GetHashCode()} Data: {callBack.Param2}");
            if (Actions.ContainsKey(callBack.Param1))
                Actions[callBack.Param1].Invoke(callBack.Param2);
        }

        public virtual void Receive<T>(Action<IMessage> @delegate)
        {
            Manager.Receive<T>(@delegate);
        }

        public virtual void Receive<T>(Action<T, IMessage> @delegate)
        {
            Manager.Receive(@delegate);
        }
    }
}