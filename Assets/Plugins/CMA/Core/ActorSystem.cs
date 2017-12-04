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

using System.Collections.Concurrent;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace CMA
{
    public class ActorSystem : IReceiver
    {
        protected ConcurrentDictionary<string, ICluster> Clusters = new ConcurrentDictionary<string, ICluster>();

        protected ConcurrentDictionary<string, ConcurrentQueue<IMessage>> MessagesWaitings =
            new ConcurrentDictionary<string, ConcurrentQueue<IMessage>>();

        protected IThreadController ThreadController;

        public ActorSystem()
        {
            ThreadController = new SingleThreadController();
        }

        public ActorSystem(IThreadController threadController)
        {
            ThreadController = threadController;
        }

        public void PushMessage(IMessage message)
        {
            if (Clusters.ContainsKey(message.Cluster))
            {
                Clusters[message.Cluster].Send(message);
            }
            else
            {
                if (!MessagesWaitings.ContainsKey(message.Cluster))
                    MessagesWaitings.TryAdd(message.Cluster, new ConcurrentQueue<IMessage>());

                MessagesWaitings[message.Cluster].Enqueue(message);
            }
        }

        public void AddCluster(ICluster cluster)
        {
            if (Clusters.ContainsKey(cluster.Name))
                Debug.LogWarning("Contains Same key");
            cluster.OnAdd(this);
            Clusters[cluster.Name] = cluster;

            if (MessagesWaitings.ContainsKey(cluster.Name))
            {
                var queue = MessagesWaitings[cluster.Name];
                while (!queue.IsEmpty)
                {
                    IMessage message = null;
                    queue.TryDequeue(out message);
                    cluster.PushMessage(message);
                }
            }
        }

        public void RemoveCluster(ICluster cluster)
        {
            if (Clusters.ContainsKey(cluster.Name))
                Clusters.TryRemove(cluster.Name, out cluster);
        }

        public bool Contains(string adress)
        {
            return Clusters.ContainsKey(adress);
        }

        public ICluster RequestDeliverycluster(string adress)
        {
            ICluster result = null;
            Clusters.TryGetValue(adress, out result);
            return result;
        }
    }
}