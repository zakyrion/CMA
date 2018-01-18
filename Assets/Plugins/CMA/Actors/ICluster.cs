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
using CMA.Messages;

namespace CMA
{
    public interface ICluster : IReceiver
    {
        ActorSystem System { get; }
        string Name { get; }
        void OnAdd(ActorSystem system);
        void AddActor(IActor actor, string adress);
        void ReplaceActor(string oldAdress, string newAdress);
        void RemoveActor(string adress);
        void RemoveActor(IActor actor);

        void Send(IMessage message);
        void Send(string condition, object data);
        void Send(object data, string adress, string cluster = "");

        void AddGlobalCall(string condition, IActor actor);
        void RemoveGlobalCall(string condition, IActor actor);

        void AskDeliveryHelper(Action<IDeliveryHelper> callback, string adress, string cluster,
            EDeliveryType deliveryType);

        void Quit();
    }
}