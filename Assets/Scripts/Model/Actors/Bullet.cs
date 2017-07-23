using CMA;
using CMA.Messages;

namespace Model
{
    public class Bullet : Actor<int>
    {
        public Bullet(int key) : base(key)
        {
        }

        public Bullet(int key, IMessageManager manager) : base(key, manager)
        {
        }

        protected override void Subscribe()
        {
        }
    }
}