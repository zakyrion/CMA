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
using System.Linq;

namespace CMA
{
    public class Adress : IAdress
    {
        private readonly List<string> _splitedAdress = new List<string>();

        public Adress(string adress)
        {
            ChangeAdress(adress);
        }

        public int Parts => _splitedAdress.Count;
        public string this[int index] => _splitedAdress != null && _splitedAdress.Count < index ? _splitedAdress[index] : null;
        public string LastPart => _splitedAdress?[_splitedAdress.Count - 1];
        public string AdressFull { get; private set; }

        public bool Contains(IAdress adress)
        {
            return Contains(adress.AdressFull);
        }

        public bool ContainsFirstPart(IAdress adress)
        {
            var part = adress[0];
            return _splitedAdress.Any(name => part == name);
        }

        public bool Contains(string adress)
        {
            return AdressFull.Contains(adress);
        }

        public int ContainsParts(IAdress adress)
        {
            var result = -1;
            var findedIndex = 0;

            for (var i = 0; i < adress.Parts; i++)
                for (var j = findedIndex; j < Parts; j++)
                    if (this[j] == adress[i])
                    {
                        findedIndex = j;
                        result = i;
                    }

            return result;
        }

        public void ChangeAdress(string newAdress)
        {
            AdressFull = newAdress;

            var index = AdressFull.Length - 1;

            while (AdressFull[index] == '/' && index > 0)
                index--;

            if (index != AdressFull.Length - 1)
                AdressFull = AdressFull.Substring(0, index);

            SplitAdress();
        }

        public void AddAdressToBack(string adressPart)
        {
            if (!string.IsNullOrEmpty(AdressFull) && !string.IsNullOrWhiteSpace(AdressFull))
            {
                var index = 0;

                while (adressPart[index] == '/' && index > 0)
                    index--;

                if (index != adressPart.Length - 1)
                    adressPart = adressPart.Substring(0, index);

                AdressFull = $"{adressPart}/{AdressFull}";

                SplitAdress();
            }
        }

        public void AddAdressToForward(string adressPart)
        {
            if (!string.IsNullOrEmpty(AdressFull) && !string.IsNullOrWhiteSpace(AdressFull))
            {
                var index = 0;

                while (adressPart[index] == '/' && index < adressPart.Length)
                    index++;

                if (index != 0)
                    adressPart = adressPart.Substring(index, adressPart.Length - 1 - index);

                AdressFull = $"{AdressFull}/{adressPart}";

                SplitAdress();
            }
        }

        protected void SplitAdress()
        {
            if (!string.IsNullOrEmpty(AdressFull) && !string.IsNullOrWhiteSpace(AdressFull))
            {
                _splitedAdress.Clear();
                var subStrs = AdressFull.Split('/');

                foreach (var str in subStrs)
                    _splitedAdress.Add(str);
            }
        }
    }
}