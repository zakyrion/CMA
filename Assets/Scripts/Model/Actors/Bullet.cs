using Akka.Actor;

namespace Model
{
    public class Bullet : ReceiveActor
    {
        private View.Bullet _bullet;

        public Bullet()
        {
            Receive<View.Bullet>(bullet => _bullet = bullet);
            Receive<DestroyBullet>(OnDestroyBullet);
        }

        private bool OnDestroyBullet(DestroyBullet destroyBullet)
        {
            _bullet.Tell(new View.Bullet.Die());
            Context.Stop(Self);
            return true;
        }

        public class DestroyBullet
        {
        }
    }
}