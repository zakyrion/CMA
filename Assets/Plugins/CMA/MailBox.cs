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
//   limitations under the License.using System.Collections;

using System.Collections.Generic;
using CMA.Core;
using CMA.Messages;

namespace CMA
{
    public class MailBox : IMailBox
    {
        protected IActor Actor;
        protected Queue<IMessage> ForActor;
        protected object Lock = new object();
        protected IMessageManager MessageManager;
        protected IThreadController ThreadController;

        public MailBox(IAdress adress)
        {
            Adress = adress;
            ThreadController = new ThreadPoolController();
            MessageManager = new MessageManager(ThreadController);

            Subscribe();
        }

        public MailBox(IAdress adress, IThreadController threadController)
        {
            Adress = adress;
            ThreadController = threadController;
            MessageManager = new MessageManager(ThreadController);

            Subscribe();
        }

        public Dictionary<string, IMailBox> Cache { get; protected set; } = new Dictionary<string, IMailBox>();

        public IMailBox Parent { get; set; }

        public List<IMailBox> Children { get; protected set; } = new List<IMailBox>();
        public IAdress Adress { get; protected set; }

        public void SendMail(IMessage message)
        {
            ThreadController.Invoke(MessageHandler, message);
        }

        public void AddChild(IMailBox mailBox)
        {
            ThreadController.Invoke(AddChildHandler, mailBox);
        }

        public void RemoveChild(IMailBox mailBox)
        {
            ThreadController.Invoke(RemoveChildHandler, mailBox);
        }

        protected void AddChildHandler(IMailBox mailBox)
        {
            mailBox.Parent = this;
            Children.Add(mailBox);
            Cache.Add(mailBox.Adress.LastPart, mailBox);
        }

        protected void RemoveChildHandler(IMailBox mailBox)
        {
            if (Cache.ContainsKey(mailBox.Adress.LastPart))
            {
                Children.Remove(mailBox);
                Cache.Remove(mailBox.Adress.LastPart);
            }
        }

        protected void MessageHandler(IMessage message)
        {
            if (string.IsNullOrEmpty(message.Adress.AdressFull))
            {
                if (Parent == null)
                {
                    message.PassCurrentAdressPart();

                    lock (Lock)
                    {
                        ForActor.Enqueue(message);
                    }

                    Actor?.CheckMailBox();

                    foreach (var child in Children)
                        child.SendMail(message);
                }
                else if (message.IsCheckFirstPath)
                {
                    lock (Lock)
                    {
                        ForActor.Enqueue(message);
                    }

                    Actor?.CheckMailBox();
                }
                else
                {
                    Parent.SendMail(message);
                }
            }
            else if (Adress.Contains(message.Adress))
            {
                if (!message.IsAdressOver && message.CurrentAdressPart == Adress.LastPart)
                {
                    if (MessageManager.CanRespounce(message))
                    {
                        MessageManager.Responce(message);
                    }
                    else
                    {
                        message.PassCurrentAdressPart();

                        lock (Lock)
                        {
                            ForActor.Enqueue(message);
                        }

                        Actor?.CheckMailBox();

                        foreach (var mailBox in Children)
                            mailBox.SendMail(message);
                    }
                }
                else
                {
                    lock (Lock)
                    {
                        ForActor.Enqueue(message);
                    }

                    Actor?.CheckMailBox();
                }
            }
            else if (message.IsCheckFirstPath)
            {
                TransmitMessage(message);
            }
            else if (message.CurrentAdressPart == Adress.LastPart)
            {
                message.PassCurrentAdressPart();
                TransmitMessage(message);
            }
            else if (Parent != null)
            {
                Parent.SendMail(message);
            }
            else if (message.Adress.ContainsFirstPart(Adress))
            {
                for (var i = 0; i < message.Adress.Parts; i++)
                    if (Adress.LastPart == message.Adress[i])
                    {
                        var mailBox = Core.Core.Get<IMailBox>($"{message.Adress[i - 1]}/{Adress.AdressFull}");

                        Adress.AddAdressToForward(message.Adress[i - 1]);

                        foreach (var child in Children)
                        {
                            var updatePath = new Message(new UpdatePath(message.Adress[i - 1]));
                            updatePath.Init(child.Adress, null);
                            child.SendMail(updatePath);
                        }

                        mailBox.AddChild(this);
                        mailBox.SendMail(message);

                        break;
                    }
            }
            else
            {
                message.Fail();
            }
        }

        protected void TransmitMessage(IMessage message)
        {
            var key = message.CurrentAdressPart;
            if (Cache.ContainsKey(key))
            {
                message.PassCurrentAdressPart();
                Cache[key].SendMail(message);
            }
            else
            {
                var mailBox = Core.Core.Get<IMailBox>($"{Adress.AdressFull}/{key}");
                AddChild(mailBox);
                mailBox.SendMail(message);
            }
        }

        protected virtual void Subscribe()
        {
            MessageManager.Receive<AddActor>(OnAddActor);
            MessageManager.Receive<UpdatePath>(OnUpdatePath);
            MessageManager.Receive<Kill>(OnKill);
        }

        private void OnKill(IMessage obj)
        {
            Parent?.RemoveChild(this);

            MessageManager.Quit();

            if (Actor != null)
            {
                lock (Lock)
                {
                    ForActor.Clear();
                    ForActor.Enqueue(obj);
                }

                Actor.CheckMailBox();
            }

            foreach (var child in Children)
            {
                var childUpdatePath = new Message(new Kill());
                childUpdatePath.Init(child.Adress, null);
                child.SendMail(childUpdatePath);
            }

            ThreadController.Remove();
        }

        private void OnUpdatePath(UpdatePath updatePath, IMessage message)
        {
            Adress.AddAdressToForward(updatePath.Data);

            foreach (var child in Children)
            {
                var childUpdatePath = new Message(new UpdatePath(updatePath.Data));
                childUpdatePath.Init(child.Adress, null);
                child.SendMail(childUpdatePath);
            }
        }

        private void OnAddActor(AddActor addActor, IMessage message)
        {
            Actor = addActor.Data;
            Actor.OnAdd(this, RequestMessage);
        }

        private IMessage[] RequestMessage()
        {
            lock (Lock)
            {
                var result = ForActor.ToArray();
                ForActor.Clear();
                return result;
            }
        }
    }
}