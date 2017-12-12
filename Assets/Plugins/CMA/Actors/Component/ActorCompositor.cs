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
using CMA.Core;

namespace CMA
{
    public class ActorCompositor : Actor, ICompositor
    {
        protected List<IComponent> ComponentList = new List<IComponent>();
        protected Dictionary<Type, List<IComponent>> Components = new Dictionary<Type, List<IComponent>>();

        public ActorCompositor()
        {
        }

        public ActorCompositor(IThreadController threadController) : base(threadController)
        {
        }

        public virtual void AddComponent(IComponent component)
        {
            var key = component.GetType();

            if (!Components.ContainsKey(key))
                Components[key] = new List<IComponent>();

            ComponentList.Add(component);
            Components[key].Add(component);
            component.OnAdd(this);
        }

        public virtual void RemoveComponent(IComponent component)
        {
            var key = component.GetType();

            if (Components.ContainsKey(key))
            {
                ComponentList.Remove(component);
                Components[key].Remove(component);
                component.OnRemove();
            }
        }

        public virtual T GetComponent<T>() where T : IComponent
        {
            var key = typeof(T);

            if (Components.ContainsKey(key))
                return (T) Components[key][0];

            foreach (var component in ComponentList)
                if (component is T)
                    return (T) component;

            return default(T);
        }
    }
}