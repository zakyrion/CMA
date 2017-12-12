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
    public class Cluster : ICluster
    {
        protected Dictionary<string, IActor> Actors = new Dictionary<string, IActor>();
        protected Dictionary<string, List<IActor>> Children = new Dictionary<string, List<IActor>>();
        protected Dictionary<string, Queue<IMessage>> MailsWaiting = new Dictionary<string, Queue<IMessage>>();
        protected MessageManager MessageManager;
        protected IThreadController ThreadController;

        public Cluster(string name)
        {
            Name = name;
            ThreadController = new SingleThreadController();
            MessageManager = new MessageManager(ThreadController);
        }

        public Cluster(string name, IThreadController threadController)
        {
            Name = name;
            ThreadController = threadController;
            MessageManager = new MessageManager(ThreadController);
        }

        public ActorSystem System { get; protected set; }
        public string Name { get; protected set; }

        public void OnAdd(ActorSystem system)
        {
            System = system;
            Subscribe();
        }

        public void AddActor(IActor actor, string adress)
        {
            ThreadController.Invoke(AddActorHandler, new Container<IActor, string>(actor, adress));
        }

        public void ReplaceActor(string oldAdress, string newAdress)
        {
            ThreadController.Invoke(ReplaceActorHandler, new Container<string, string>(oldAdress, newAdress));
        }

        public void RemoveActor(string adress)
        {
            ThreadController.Invoke(RemoveActorHandler, adress);
        }

        public void RemoveActor(IActor actor)
        {
            ThreadController.Invoke(RemoveActorHandler, actor.Adress);
        }

        public void Send(IMessage message)
        {
            ThreadController.Invoke(SendHandler, message);
        }

        public void AskDeliveryHelper(Action<IDeliveryHelper> callback, string adress, string cluster,
            EDeliveryType deliveryType)
        {
            ThreadController.Invoke(AskDeliveryHelperHandler,
                new ReceiversAsk(callback, adress, cluster, deliveryType));
        }

        public void Quit()
        {
            ThreadController.Invoke(QuitHandler);
        }

        public void PushMessage(IMessage message)
        {
            Send(message);
        }

        protected void ReplaceActorHandler(Container<string, string> container)
        {
            if (Actors.ContainsKey(container.ParamOne))
            {
                var actor = Actors[container.ParamOne];
                Actors.Remove(container.ParamOne);
                actor.Quit();

                var parent = GetParentAdress(container.ParamOne);
                if (parent != null)
                    Children[parent].Remove(actor);

                AddActorHandler(new Container<IActor, string>(actor,container.ParamTwo));
            }
        }

        public void QuitHandler()
        {
            System.RemoveCluster(this);
            foreach (var kvp in Actors)
                kvp.Value.Quit();

            Actors.Clear();
            Children.Clear();
        }

        protected void AskDeliveryHelperHandler(ReceiversAsk ask)
        {
            if (ask.Cluster == null || ask.Cluster == Name)
            {
                var receivers = new List<IReceiver>();

                if (ask.DeliveryType != EDeliveryType.ToChildern && Actors.ContainsKey(ask.Adress))
                    receivers.Add(Actors[ask.Adress]);

                if (ask.DeliveryType != EDeliveryType.ToClient && Children.ContainsKey(ask.Adress))
                    receivers.AddRange(Children[ask.Adress]);

                ask.Callback(new DeliveryHelper(receivers));
            }
            else
            {
                Debug.Log("Add Sending to another clusters");
            }
        }

        protected virtual void SendHandler(IMessage message)
        {
            if (message.Cluster == null || message.Cluster == Name)
            {
                if (string.IsNullOrEmpty(message.Adress))
                {
                    MessageManager.Responce(message);
                }
                else
                {
                    if (Actors.ContainsKey(message.Adress))
                    {
                        if (message.DeliveryType != EDeliveryType.ToChildern)
                        {
                            var actor = Actors[message.Adress];
                            actor.PushMessage(message);
                        }

                        if (message.DeliveryType != EDeliveryType.ToClient)
                        {
                            var children = Children[message.Adress];
                            for (var i = 0; i < children.Count; i++)
                                children[i].PushMessage(message);
                        }
                    }
                    else
                    {
                        if (!MailsWaiting.ContainsKey(message.Adress))
                            MailsWaiting.Add(message.Adress, new Queue<IMessage>());

                        MailsWaiting[message.Adress].Enqueue(message);
                    }
                }
            }
            else
            {
                message.SetBackCluster(Name);
                System.PushMessage(message);
            }
        }

        protected void RemoveActorHandler(string adress)
        {
            if (Actors.ContainsKey(adress))
            {
                var actor = Actors[adress];
                Actors.Remove(adress);
                actor.Quit();

                var parent = GetParentAdress(adress);
                if (parent != null)
                    Children[parent].Remove(actor);
            }
        }

        public static string GetParentAdress(string adress)
        {
            var lastIndex = adress.LastIndexOf("/", StringComparison.Ordinal);
            if (lastIndex > 0)
                return adress.Substring(0, lastIndex);
            return null;
        }

        protected void AddActorHandler(Container<IActor, string> container)
        {
            Actors.Add(container.ParamTwo, container.ParamOne);

            var parent = GetParentAdress(container.ParamTwo);
            if (parent != null)
            {
                if (!Children.ContainsKey(parent))
                    Children[parent] = new List<IActor>();
                Children[parent].Add(container.ParamOne);
            }

            if (MailsWaiting.ContainsKey(container.ParamTwo))
            {
                var messages = MailsWaiting[container.ParamTwo];
                while (messages.Count > 0)
                    SendHandler(messages.Dequeue());
            }

            container.ParamOne.OnAdd(this, container.ParamTwo);
        }

        protected virtual void Subscribe()
        {
            Receive<AddActor>(OnAddActor);
        }

        private void OnAddActor(AddActor addActor, IMessage message)
        {
            AddActorHandler(new Container<IActor, string>(addActor.Param1, addActor.Param2));
        }

        public virtual void Receive<T>(Action<IMessage> @delegate)
        {
            MessageManager.Receive<T>(@delegate);
        }

        public virtual void Receive<T>(Action<T, IMessage> @delegate)
        {
            MessageManager.Receive(@delegate);
        }

        protected struct Container<T, K>
        {
            public T ParamOne;
            public K ParamTwo;

            internal Container(T one, K two) : this()
            {
                ParamOne = one;
                ParamTwo = two;
            }
        }

        protected struct ReceiversAsk
        {
            public Action<IDeliveryHelper> Callback;
            public string Adress;
            public string Cluster;
            public EDeliveryType DeliveryType;

            public ReceiversAsk(Action<IDeliveryHelper> callback, string adress, string cluster,
                EDeliveryType deliveryType)
            {
                Callback = callback;
                Adress = adress;
                Cluster = cluster;
                DeliveryType = deliveryType;
            }
        }
    }
}