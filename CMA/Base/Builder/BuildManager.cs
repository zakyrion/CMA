﻿//   Copyright {CMA} {Kharsun Sergei}
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

namespace CMA
{
    public class BuildManager: IBuildManager
    {
        protected Dictionary<string, IBuilder> Builders = new Dictionary<string, IBuilder>();

        public virtual void SubscribeBuilder(IBuilder builder)
        {
            Builders[builder.Key] = builder;
        }

        public virtual T Build<T>()
        {
            T result = default(T);
            string key = typeof(T).Name;

            if (Builders.ContainsKey(key))
                result = (T) Builders[key].Build();

            return result;
        }

        public T Build<T>(object param)
        {
            T result = default(T);
            string key = typeof(T).Name;

            if (Builders.ContainsKey(key))
                result = (T)Builders[key].Build(param);

            return result;
        }
    }
}