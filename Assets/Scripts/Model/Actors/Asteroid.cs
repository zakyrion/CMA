using CMA;
using CMA.Messages;

namespace Model
{
    public class Asteroid : Actor<int>
    {
        public Asteroid(int key) : base(key)
        {
        }

        public Asteroid(int key, IMessageManager manager) : base(key, manager)
        {
        }

        protected override void Subscribe()
        {
        }
    }
}