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

using CMA.Core;
using CMA.Messages;

namespace CMA
{
    public abstract class MonoCompositor : MonoActor, ICompositor
    {
        protected ActorCompositor Compositor;

        public void AddComponent(IComponent component)
        {
            Compositor.AddComponent(component);
        }

        public void RemoveComponent(IComponent component)
        {
            Compositor.RemoveComponent(component);
        }

        T ICompositor.GetComponent<T>()
        {
            return Compositor.GetComponent<T>();
        }

        protected override void Awake()
        {
            Actor = Compositor = new ActorCompositor(new MainThreadController());
            Receive<Kill>(OnKill);
            Subscribe();
        }
    }
}