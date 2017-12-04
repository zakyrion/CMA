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
    public abstract class Communication : ICommunication
    {
        public bool IsAbsAdress { get; protected set; }

        public EDeliveryType DeliveryType { get; protected set; } = EDeliveryType.ToAll;
        public string Adress { get; protected set; }
        public string BackAdress { get; protected set; }
        public string Cluster { get; protected set; }
        public string BackCluster { get; protected set; }

        public void Init(string adress, string backAdress)
        {
            BackAdress = backAdress;
            ParseAdress(adress);
        }

        public void SetAdress(string adress, string cluster = null)
        {
            ParseAdress(adress);
            Cluster = cluster;
        }

        public void SetCluster(string cluster)
        {
            Cluster = cluster;
        }

        public void SetBackAdress(string backAdress, string cluster = null)
        {
            BackAdress = backAdress;
            BackCluster = cluster;
        }

        public void SetBackCluster(string cluster)
        {
            BackCluster = cluster;
        }

        protected void ParseAdress(string adress)
        {
            Adress = adress;

            if (!string.IsNullOrEmpty(Adress))
                if (Adress[Adress.Length - 1] == '*')
                {
                    DeliveryType = EDeliveryType.ToChildern;
                    Adress = Adress.Substring(0, Adress.Length - 2);
                }
                else if (Adress[Adress.Length - 1] == '!')
                {
                    DeliveryType = EDeliveryType.ToClient;
                    Adress = Adress.Substring(0, Adress.Length - 2);
                }
        }
    }
}