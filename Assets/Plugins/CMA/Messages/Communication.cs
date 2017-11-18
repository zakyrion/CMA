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
    public abstract class Communication : ICommunication
    {
        private IAdress _adressRef;
        private IAdress _backAdressRef;
        private int _index;
        protected List<string> Traces = new List<string>();


        public bool IsAbsAdress { get; protected set; }
        public EDeliveryType DeliveryType { get; protected set; } = EDeliveryType.ToAll;

        public IAdress BackAdress
        {
            get { return _backAdressRef = _backAdressRef ?? new Adress(BackAdressFull); }
        }

        public IAdress Adress
        {
            get { return _adressRef = _adressRef ?? new Adress(AdressFull); }
        }

        public string AdressFull { get; protected set; }
        public string BackAdressFull { get; protected set; }
        public string CurrentAdressPart => Adress[_index];

        public void Init(string adress, string backAdress)
        {
            BackAdressFull = backAdress;

            SetAdress(adress);
        }

        public void Init(IAdress adress, string backAdress)
        {
            AdressFull = adress.AdressFull;
            _adressRef = adress;
            IsAbsAdress = adress.IsAbsAdress;

            DeliveryType = _adressRef.DeliveryType;
            BackAdressFull = backAdress;
        }

        public void SetAdress(string adress)
        {
            AdressFull = adress;

            if (!string.IsNullOrEmpty(AdressFull))
            {
                IsAbsAdress = AdressFull[0] == '*';

                if (IsAbsAdress)
                    AdressFull = AdressFull.Remove(0, 1);

                if (AdressFull[AdressFull.Length - 1] == '*')
                {
                    DeliveryType = EDeliveryType.ToChildern;
                    AdressFull = AdressFull.Substring(0, AdressFull.Length - 2);
                }
                else if (AdressFull[AdressFull.Length - 1] == '!')
                {
                    DeliveryType = EDeliveryType.ToClient;
                    AdressFull = AdressFull.Substring(0, AdressFull.Length - 2);
                }
            }
        }

        public void SetAdress(IAdress adress)
        {
            AdressFull = adress.AdressFull;
            _adressRef = adress;
            IsAbsAdress = adress.IsAbsAdress;
            DeliveryType = adress.DeliveryType;
        }

        public void SetBackAdress(string backAdress)
        {
            _backAdressRef = null;
            BackAdressFull = backAdress;
        }

        public void PassCurrentAdressPart()
        {
            _index++;
        }

        public void PassAdressFull()
        {
            _index = Adress.Parts;
        }

        public bool IsCheckFirstPath => _index > 0;
        public bool IsAdressOver => _index >= Adress.Parts;
        public bool IsFaild { get; protected set; }

        public virtual void Fail()
        {
            IsFaild = true;
        }

        public void AddTrace(string trace)
        {
            Traces.Add(trace);
        }

        public List<string> Trace()
        {
            return Traces;
        }
    }
}