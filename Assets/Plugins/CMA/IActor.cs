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

using System;
using CMA.Messages;

namespace CMA
{
    public interface IActor
    {
        IMailBox MailBox { get; }

        void CheckMailBox();
        void OnAdd(IMailBox mailBox, Func<IMessage[]> messagesRequest);
        void Quit();

        void Send(object data, string adress = "");
        void Send(object data, Action action, string adress = "");

        void Ask<TR>(Action<TR> action, string adress = "");
        void Ask<TM, TR>(TM data, Action<TR> action, string adress = "");

        void Respounce(IMessage message, object data = null);
    }
}