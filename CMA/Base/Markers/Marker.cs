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

namespace CMA.Markers
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Type of Object Key</typeparam>
    public abstract class Marker<T> : IMarker
    {
        /// <summary>
        /// </summary>
        /// <param name="key">Object Key or id for identefication</param>
        protected Marker(T key)
        {
            Key = key;
            ObjKeyType = typeof(T).ToString();
        }

        public virtual T Key { get; protected set; }

        public object ObjKey
        {
            get { return Key; }
        }

        public virtual string MarkerKey
        {
            get { return GetType().ToString(); }
        }

        public string ObjKeyType { get; protected set; }

        public bool IsCheck { get; protected set; }

        public void Check()
        {
            IsCheck = true;
        }
    }
}