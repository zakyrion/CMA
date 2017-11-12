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

using System;
using System.Collections.Generic;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace CMA
{
    public class MailBox : IMailBox
    {
        private readonly List<IMessage> _forParent = new List<IMessage>();

        private IMailBox _parent;
        protected Queue<IMessage> ForActor = new Queue<IMessage>();

        protected object Lock = new object();
        protected IMessageManager MessageManager;
        protected IThreadController ThreadController;

        public MailBox(IAdress adress)
        {
            Adress = adress;
            ThreadController = new ThreadPoolController();
            MessageManager = new MessageManager(ThreadController);

            Subscribe();
            Debug.Log("Create MailBox: " + adress.AdressFull);
        }

        public MailBox(IAdress adress, IThreadController threadController)
        {
            Adress = adress;
            ThreadController = threadController;
            MessageManager = new MessageManager(ThreadController);

            Subscribe();
            Debug.Log("Create MailBox: " + adress.AdressFull);
        }

        public Dictionary<string, IMailBox> Cache { get; protected set; } = new Dictionary<string, IMailBox>();
        public IActor Actor { get; protected set; }

        public IMailBox Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                ThreadController.Invoke(OnAddParentHandler);
            }
        }

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

        public void AddActor(IActor actor, string adress = null)
        {
            ThreadController.Invoke(AddActorHandler, new Tuple<IActor, string>(actor, adress));
        }

        private void OnAddParentHandler()
        {
            foreach (var message in _forParent)
                _parent.SendMail(message);
        }

        protected void AddActorHandler(Tuple<IActor, string> tuple)
        {
            Debug.Log($"AddActorHandler: {tuple.Item2} to: {Adress.AdressFull}");
            var addActor = new AddActor(tuple.Item1);

            if (string.IsNullOrEmpty(tuple.Item2))
            {
                OnAddActor(addActor, null);
            }
            else
            {
                var message = new Message(addActor);
                message.Init(new Adress(tuple.Item2), Adress);
                SendMail(message);
            }
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
            try
            {
                message.AddTrace(Adress.AdressFull);

                if (string.IsNullOrEmpty(message.Adress.AdressFull))
                {
                    EmptyAdressMessageHandler(message);
                }
                else if (Adress.IsDestination(message.Adress))
                {
                    AtDestinationMessageHandler(message);
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
                    _forParent.Add(message);
                    /*for (var i = 0; i < message.Adress.Parts; i++)
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
                        }*/
                }
                else
                {
                    message.Fail();
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Exception: {e}");
            }
        }

        protected void AtDestinationMessageHandler(IMessage message)
        {
            if (!message.IsAdressOver)
            {
                message.PassAdressFull();

                if (MessageManager.CanRespounce(message))
                {
                    MessageManager.Responce(message);
                }
                else if (message.Adress.ForChildren)
                {
                    foreach (var mailBox in Children)
                        mailBox.SendMail(message);
                }
                else
                {
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

        protected void EmptyAdressMessageHandler(IMessage message)
        {
            if (Parent == null)
            {
                Debug.Log($"Catch: {message.GetKey()} at: {Adress.AdressFull}");
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
                Debug.Log($"Catch: {message.GetKey()} at: {Adress.AdressFull}");
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

        protected void TransmitMessage(IMessage message)
        {
            var key = message.CurrentAdressPart;

            if (key == null)
            {
                Debug.Log($"{message.GetKey()} Current Adress Null, Full Adress: {message.Adress}");
                message.ShowTrace();
            }

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
            MessageManager.Receive<CallBack>(OnCallback);
        }

        private void OnCallback(IMessage obj)
        {
            lock (Lock)
            {
                ForActor.Enqueue(obj);
            }

            Actor?.CheckMailBox();
        }

        private void OnKill(IMessage obj)
        {
            if (!obj.Adress.ForChildren)
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

                ThreadController.Remove();
            }
            else
            {
                Debug.Log("Catch Kill all children");
            }

            foreach (var child in Children)
            {
                var childUpdatePath = new Message(new Kill());
                childUpdatePath.Init(child.Adress, null);
                child.SendMail(childUpdatePath);
            }
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
            Debug.Log($"Actor Added to: {Adress.AdressFull}");

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