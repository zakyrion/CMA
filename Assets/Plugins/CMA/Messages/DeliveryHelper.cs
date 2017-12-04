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
using CMA.Messages;

namespace CMA
{
    public class DeliveryHelper : IDeliveryHelper
    {
        private readonly List<IReceiver> _receivers;

        public DeliveryHelper(List<IReceiver> receivers)
        {
            _receivers = receivers;
        }

        public void PushMessage(object data, string backAdress = null, string backCluster = null)
        {
            for (var i = 0; i < _receivers.Count; i++)
            {
                var message = new Message(data);
                message.SetBackAdress(backAdress, backCluster);
                var index = i;
                _receivers[index].PushMessage(message);
            }
        }
    }
}