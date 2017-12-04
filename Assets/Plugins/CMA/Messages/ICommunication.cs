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

namespace CMA.Messages
{
    public interface ICommunication
    {
        EDeliveryType DeliveryType { get; }

        string Adress { get; }
        string BackAdress { get; }
        string Cluster { get; }
        string BackCluster { get; }

        void Init(string adress, string backAdress);
        void SetAdress(string adress, string cluster = null);
        void SetCluster(string cluster);
        void SetBackAdress(string backAdress, string cluster = null);
        void SetBackCluster(string cluster);
    }
}