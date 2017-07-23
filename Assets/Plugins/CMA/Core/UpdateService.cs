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
using UnityEngine;

namespace CMA.Core
{
    [HideInInspector]
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        private readonly HashSet<IUpdated> _hashSet = new HashSet<IUpdated>();
        private readonly List<IUpdated> _updateds = new List<IUpdated>();
        private static UpdateService _updateService;
        public static UpdateService Instance { get { return _updateService ?? (_updateService = Create()); } }

        static UpdateService Create()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.name = "UpdateService";
            obj.GetComponent<Renderer>().enabled = false;
            obj.GetComponent<Collider>().enabled = false;
            var updateService = obj.AddComponent<UpdateService>();
            return updateService;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Add(IUpdated updated)
        {
            if (!_hashSet.Contains(updated))
            {
                _hashSet.Add(updated);
                _updateds.Add(updated);
            }
        }

        public void Remove(IUpdated updated)
        {
            if (_hashSet.Contains(updated))
            {
                _hashSet.Remove(updated);
                _updateds.Remove(updated);
            }
        }

        protected virtual void Update()
        {
            for (var index = 0; index < _updateds.Count; index++)
                _updateds[index].Update();
        }
    }
}