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
using System.Linq;
using CMA.Markers;

namespace CMA.Messages
{
    public abstract class Communication : ICommunication
    {
        protected Dictionary<string, IMarker> Cache = new Dictionary<string, IMarker>();
        protected Dictionary<string, IMarker> CacheReturningMarkers = new Dictionary<string, IMarker>();
        protected HashSet<long> Ids = new HashSet<long>();
        protected List<string> Traces = new List<string>();

        protected Communication()
        {
            Markers = new List<IMarker>();
            ReturningMarkers = new List<IMarker>();
        }

        public bool IsAllMarkersCheck()
        {
            return Markers.All(marker => marker.IsCheck);
        }

        public bool IsContainsActorId(long id)
        {
            return Ids.Contains(id);
        }

        public bool IsFaild { get; protected set; }
        public List<IMarker> Markers { get; protected set; }
        public List<IMarker> ReturningMarkers { get; protected set; }

        public T GetMarker<T>() where T : IMarker
        {
            var key = typeof(T).ToString();
            return (T) Cache[key];
        }

        public T GetReturningMarker<T>() where T : IMarker
        {
            var key = typeof(T).ToString();
            return (T) CacheReturningMarkers[key];
        }

        public bool Contains<T>()
        {
            var key = typeof(T).ToString();
            return Cache.ContainsKey(key);
        }

        public void AddMarkerForReturn(IMarker marker)
        {
            ReturningMarkers.Add(marker);
            CacheReturningMarkers[marker.MarkerKey] = marker;
        }

        public void AddMarker(IMarker marker)
        {
            Markers.Add(marker);
            Cache[marker.MarkerKey] = marker;
        }

        public void AddMarkers(IEnumerable<IMarker> markers)
        {
            foreach (var marker in markers)
                AddMarker(marker);
        }

        public void AddActorId(long id)
        {
            Ids.Add(id);
        }

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