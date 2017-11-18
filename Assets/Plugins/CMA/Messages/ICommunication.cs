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

using System.Collections.Generic;

namespace CMA.Messages
{
    public interface ICommunication
    {
        bool IsCheckFirstPath { get; }
        bool IsAdressOver { get; }
        bool IsFaild { get; }
        bool IsAbsAdress { get; }

        EDeliveryType DeliveryType { get; }
        IAdress BackAdress { get; }
        IAdress Adress { get; }

        string AdressFull { get; }
        string BackAdressFull { get; }
        string CurrentAdressPart { get; }

        void Init(string adress, string backAdress);
        void Init(IAdress adress, string backAdress);
        void SetAdress(string adress);
        void SetAdress(IAdress adress);
        void SetBackAdress(string backAdress);
        void PassCurrentAdressPart();
        void PassAdressFull();

        void Fail();
        void AddTrace(string trace);
        List<string> Trace();
    }
}