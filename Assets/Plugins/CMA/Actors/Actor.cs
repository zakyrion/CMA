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

        public ICluster Cluster { get; protected set; }
        public IThreadController ThreadController { get; protected set; }
        public string Adress { get; protected set; }
        public string Parent { get; protected set; }

        public virtual void OnAdd(ICluster cluster, string adress)
        {
            //Debug.Log($"Add Actor to Cluster: {cluster.Name} By Adress: {adress}");
            Cluster = cluster;
            Adress = adress;
            Parent = CMA.Cluster.GetParentAdress(Adress);
            foreach (var message in _forSend)
            {
                /*if (string.IsNullOrEmpty(mailBox.Adress.AdressFull))
                    Debug.Log($"{GetType()}");*/
                message.SetBackAdress(Adress, cluster.Name);
                Cluster.Send(message);
            }

            _forSend.Clear();
        }

        public virtual void Quit()
        {
            if (!_isQuit)
            {
                _isQuit = true;
                Cluster.RemoveActor(Adress);
                ThreadController.Remove();
                Manager.Quit();
            }
        }

        public virtual void Send(object data, string adress, string cluster = null)
        {
            var message = new Message(data);

            if (Cluster != null)
            {
                message.Init(adress, Adress);
                message.SetCluster(cluster);
                Cluster.Send(message);
            }
            else
            {
                message.SetAdress(adress,cluster);
                _forSend.Add(message);
            }

            //Debug.Log($"Send message: {message.Key} from: {Adress} to: {message.Adress}");
        }

        public virtual void Send(object data, Action action, string adress, string cluster = null)
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


            if (Cluster != null)
            {
                message.Init(adress, Adress);
                message.SetCluster(cluster);
                Cluster.Send(message);
            }
            else
            {
                message.SetAdress(adress, cluster);
                _forSend.Add(message);
            }

            //Debug.Log($"Send message: {message.Key} from: {Adress} to: {message.Adress}");
        }

        public void Ask<TR>(Action<TR> action, string adress, string cluster = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new SimpleRequest<TR>(code);

            if (Cluster != null)
            {
                request.Init(adress, Adress);
                request.SetCluster(cluster);
                Cluster.Send(request);
            }
            else
            {
                request.SetAdress(adress, cluster);
                _forSend.Add(request);
            }

            //Debug.Log($"Send request: {request.Key} from: {Adress} to: {request.Adress}");
        }

        public virtual void Ask<TM, TR>(TM data, Action<TR> action, string adress, string cluster = "")
        {
            IActionHandler handler = new ActionHandler<TR>(action);
            IRespounceCode code = new RespounceCode(RespounceId);

            Actions.Add(code, handler);

            var request = new Request<TR>(data, code);


            if (Cluster != null)
            {
                request.Init(adress, Adress);
                request.SetCluster(cluster);
                Cluster.Send(request);
            }
            else
            {
                request.SetAdress(adress);
                request.SetAdress(adress, cluster);
                _forSend.Add(request);
            }

            //Debug.Log($"Send request: {request.Key} from: {Adress} to: {request.Adress}");
        }

        public void PushMessage(IMessage message)
        {
            Manager.Responce(message);
        }

        public void Respounce(IMessage message, object data = null)
        {
            //Debug.Log($"At: {Adress} Respounce: {message.Key} by Adress: {message.BackAdress}");

            var callback = new Message(new CallBack(message.RespounceCode, data));

            if (Cluster != null)
            {
                callback.Init(message.BackAdress, Adress);
                callback.SetCluster(message.BackCluster);
                Cluster.Send(callback);
            }
            else
            {
                callback.SetAdress(message.BackAdress, message.BackCluster);
                _forSend.Add(callback);
            }
        }

        protected void AskDeliveryHelper(string adress, string cluster, EDeliveryType deliveryType,
            Action<IDeliveryHelper> callback)
        {

            Cluster.AskDeliveryHelper(receiver => ThreadController.Invoke(callback, receiver), adress, cluster,
                deliveryType);
        }

        protected void AskDeliveryHelper(string adress, string cluster,
            Action<IDeliveryHelper> callback)
        {
            var deliveryType = EDeliveryType.ToAll;

            if (!string.IsNullOrEmpty(Adress))
                if (Adress[Adress.Length - 1] == '*')
                {
                    deliveryType = EDeliveryType.ToChildern;
                    Adress = Adress.Substring(0, Adress.Length - 2);
                }
                else if (Adress[Adress.Length - 1] == '!')
                {
                    deliveryType = EDeliveryType.ToClient;
                    Adress = Adress.Substring(0, Adress.Length - 2);
                }

            Cluster.AskDeliveryHelper(receiver => ThreadController.Invoke(callback, receiver), adress, cluster,
                deliveryType);
        }

        protected virtual void OnKill(IMessage obj)
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