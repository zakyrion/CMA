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
        private int _index;
        protected List<string> Traces = new List<string>();

        public IAdress BackAdress { get; protected set; }
        public IAdress Adress { get; protected set; }
        public string CurrentAdressPart => Adress[_index];

        public void Init(IAdress adress, IAdress backAdress)
        {
            Adress = adress;
            BackAdress = backAdress;
        }

        public void PassCurrentAdressPart()
        {
            _index++;
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