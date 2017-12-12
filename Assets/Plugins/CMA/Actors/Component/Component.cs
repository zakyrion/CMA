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
using CMA.Messages;

namespace CMA
{
    public class Component : IComponent
    {
        protected ICompositor Parent;

        public virtual void OnAdd(ICompositor actor)
        {
            Parent = actor;
            Subscribe();
        }

        public virtual void OnRemove()
        {
            Parent.Manager.RemoveByParent(this);
            Parent = null;
        }

        protected void Receive<T>(Action<IMessage> @delegate)
        {
            Parent.Receive<T>(@delegate);
        }

        protected void Receive<T>(Action<T, IMessage> @delegate)
        {
            Parent.Receive(@delegate);
        }

        protected void RemoveReceiver<T>(Action<T, IMessage> @delegate)
        {
            Parent.Manager.RemoveReceiver(@delegate);
        }

        protected void RemoveReceiver<T>(Action<IMessage> @delegate)
        {
            Parent.Manager.RemoveReceiver<T>(@delegate);
        }

        protected virtual void Subscribe()
        {
        }
    }
}