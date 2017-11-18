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
using System.Collections.Concurrent;
using System.Collections.Generic;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace CMA
{
    public class Actor : IActor
    {
        private readonly List<IMessage> _forSend = new List<IMessage>();
        private bool _isQuit;
        private int _respounceId;
        protected Dictionary<IRespounceCode, IActionHandler> Actions = new Dictionary<IRespounceCode, IActionHandler>();

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

        protected int RespounceId => _respounceId++;

        public IMessageManager Manager { get; protected set; }

        public IThreadController ThreadController { get; protected set; }
        public string Adress => MailBox?.Adress.AdressFull;
        public IMailBox MailBox { get; protected set; }

        public void OnAdd(IMailBox mailBox)
        {
            MailBox = mailBox;

            foreach (var message in _forSend)
            {
                /*if (string.IsNullOrEmpty(mailBox.Adress.AdressFull))
                    Debug.Log($"{GetType()}");*/
                message.SetBackAdress(MailBox.Adress.AdressFull);
                MailBox.PushMail(message);
            }

            _forSend.Clear();
        }

        public virtual void Quit()
        {
            if (!_isQuit)
            {
                _isQuit = true;
                ThreadController.Remove();
                Manager.Quit();
            }
        }

        public virtual void Send(object data, string adress = "")
        {
            var message = new Message(data);

            if (MailBox != null)
            {
                message.Init(adress, MailBox.Adress.AdressFull);
                MailBox.PushMail(message);
            }
            else
            {
                message.SetAdress(adress);
                _forSend.Add(message);
            }

            //Debug.Log($"Send message: {message.Key()} from: {Adress} to: {message.Adress}");
        }

        public virtual void Send(object data, IAdress adress)
        {
            var message = new Message(data);

            if (MailBox != null)
            {
                message.Init(adress, MailBox.Adress.AdressFull);
                MailBox.PushMail(message);
            }
            else
            {
                message.SetAdress(adress);
                _forSend.Add(message);
            }

            //Debug.Log($"Send message: {message.Key()} from: {Adress} to: {message.Adress}");
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
                message.Init(adress, MailBox.Adress.AdressFull);
                MailBox.PushMail(message);
            }
            else
            {
                message.SetAdress(adress);
                _forSend.Add(message);
            }

            //Debug.Log($"Send message: {message.Key()} from: {Adress} to: {message.Adress}");
        }

        public void Ask<TR>(Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new SimpleRequest<TR>(code);

            if (MailBox != null)
            {
                request.Init(adress, MailBox.Adress.AdressFull);
                MailBox.PushMail(request);
            }
            else
            {
                request.SetAdress(adress);
                _forSend.Add(request);
            }

            //Debug.Log($"Send request: {request.Key()} from: {Adress} to: {request.Adress}");
        }

        public virtual void Ask<TM, TR>(TM data, Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new Request<TR>(data, code);


            if (MailBox != null)
            {
                request.Init(adress, MailBox.Adress.AdressFull);
                MailBox.PushMail(request);
            }
            else
            {
                request.SetAdress(adress);
                _forSend.Add(request);
            }

            //Debug.Log($"Send request: {request.Key()} from: {Adress} to: {request.Adress}");
        }

        public void PushMessage(IMessage message)
        {
            Manager.Responce(message);
        }

        public void Respounce(IMessage message, object data = null)
        {
            //Debug.Log($"At: {Adress} Respounce: {message.Key()} by Adress: {message.BackAdress}");

            var callback = new Message(new CallBack(message.RespounceCode, data));

            if (MailBox != null)
            {
                callback.Init(message.BackAdress, MailBox.Adress.AdressFull);
                MailBox.PushMail(callback);
            }
            else
            {
                callback.SetAdress(message.BackAdress);
                _forSend.Add(callback);
            }
        }

        private void OnKill(IMessage obj)
        {
            if (!_isQuit)
            {
                _isQuit = true;
                ThreadController.Invoke(OnKillHandler);
            }
        }

        private void OnKillHandler()
        {
            Quit();
        }

        protected virtual void Subscribe()
        {
        }

        internal virtual void OnCallback(CallBack callBack, IMessage message)
        {
            //Debug.Log(
            //    $"At: {Adress} To: {message.Adress} Catch Callback: Hash {callBack.Param1.GetHashCode()} Data: {callBack.Param2}");
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