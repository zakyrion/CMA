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

namespace CMA
{
    public class Actor : IActor
    {
        private readonly List<IMessage> _forSend = new List<IMessage>();
        private int _id;
        protected Dictionary<IRespounceCode, IActionHandler> Actions = new Dictionary<IRespounceCode, IActionHandler>();

        protected Func<IMessage[]> MessagesRequest;
        protected IThreadController ThreadController;

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

        public string Adress => MailBox?.Adress.AdressFull;

        protected int RespounceId => _id++;

        public IMessageManager Manager { get; protected set; }
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
                MailBox.SendMail(message);

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
            message.Init(new Adress(adress), MailBox.Adress);

            if (MailBox != null)
                MailBox.SendMail(message);
            else
                _forSend.Add(message);
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

            message.Init(new Adress(adress), MailBox.Adress);

            if (MailBox != null)
                MailBox.SendMail(message);
            else
                _forSend.Add(message);
        }

        public void Ask<TR>(Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new SimpleRequest<TR>(code);

            request.Init(new Adress(adress), MailBox.Adress);

            if (MailBox != null)
                MailBox.SendMail(request);
            else
                _forSend.Add(request);
        }

        public virtual void Ask<TM, TR>(TM data, Action<TR> action, string adress = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new Request<TR>(data, code);
            request.Init(new Adress(adress), MailBox.Adress);

            if (MailBox != null)
                MailBox.SendMail(request);
            else
                _forSend.Add(request);
        }

        public void Respounce(IMessage message, object data = null)
        {
            var callback = new Message(new CallBack(message.RespounceCode, data));
            callback.Init(message.BackAdress, MailBox.Adress);

            if (MailBox != null)
                MailBox.SendMail(callback);
            else
                _forSend.Add(callback);
        }

        private void OnKill(IMessage obj)
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

        private void OnCallback(CallBack callBack, IMessage message1)
        {
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