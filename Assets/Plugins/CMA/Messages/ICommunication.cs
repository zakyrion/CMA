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
        IAdress BackAdress { get; }
        IAdress Adress { get; }
        string CurrentAdressPart { get; }

        void Init(IAdress adress, IAdress backAdress);
        void PassCurrentAdressPart();

        void Fail();
        void AddTrace(string trace);
        List<string> Trace();
    }
}