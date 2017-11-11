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

namespace CMA
{
    public interface IAdress
    {
        int Parts { get; }
        string this[int index] { get; }
        string LastPart { get; }
        string AdressFull { get; }

        bool Contains(IAdress adress);
        bool ContainsFirstPart(IAdress adress);
        bool Contains(string adress);

        int ContainsParts(IAdress adress);

        void ChangeAdress(string newAdress);
        void AddAdressToBack(string adressPart);
        void AddAdressToForward(string adressPart);
    }
}