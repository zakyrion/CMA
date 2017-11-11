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

namespace CMA
{
    public class Container<T>
    {
        public Container(T data)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }

    public class Container<T, K>
    {
        public Container(T param1, K param2)
        {
            Param1 = param1;
            Param2 = param2;
        }

        public T Param1 { get; protected set; }
        public K Param2 { get; protected set; }
    }

    public class Container<T, K, M>
    {
        public Container(T param1, K param2, M param3)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
        }

        public T Param1 { get; protected set; }
        public K Param2 { get; protected set; }
        public M Param3 { get; protected set; }
    }
}