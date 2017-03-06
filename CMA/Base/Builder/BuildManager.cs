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

        public T Build<T>(IMessage message)
        {
            T result = default(T);
            string key = typeof(T).Name;

            if (Builders.ContainsKey(key))
                result = (T)Builders[key].Build(message);

            return result;
        }
    }
}