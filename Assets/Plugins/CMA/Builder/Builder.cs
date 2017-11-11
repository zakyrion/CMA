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

using CMA.Messages;

namespace CMA
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    /// <typeparam name="K">Param Type</typeparam>
    public abstract class Builder<T, K> : IBuilder
    {
        public string Key
        {
            get { return typeof(T).Name; }
        }

        public virtual object Build()
        {
            return default(T);
        }

        public object Build(object param)
        {
            return Build((K) param);
        }

        public abstract object Build(K param);
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public abstract class Builder<T> : IBuilder
    {
        public string Key
        {
            get { return typeof(T).Name; }
        }

        public abstract object Build();

        public object Build(object param)
        {
            return Build((IMessage) param);
        }
    }
}