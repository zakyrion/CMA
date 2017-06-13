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

namespace CMA.Core
{
    public static class Core
    {
        private static readonly BuildManager _buildManager = new BuildManager();

        public static void SubscribeBuilder(IBuilder builder)
        {
            _buildManager.SubscribeBuilder(builder);
        }

        public static T Get<T>()
        {
            return _buildManager.Build<T>();
        }

        public static T Get<T>(object param)
        {
            return _buildManager.Build<T>(param);
        }
    }
}