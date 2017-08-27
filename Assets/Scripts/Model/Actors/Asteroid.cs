using Akka.Actor;
using UnityEngine;

namespace Model
{
    public class Asteroid : ReceiveActor
    {
        private View.Asteroid _asteroid;

        public Asteroid()
        {
            Receive<View.Asteroid>(asteroid =>
            {
                _asteroid = asteroid;
                Debug.Log("Catch Asteroid");
            });
            Receive<DestroyAsteroid>(OnDestroyAsteroid);
        }

        private bool OnDestroyAsteroid(DestroyAsteroid destroyAsteroid)
        {
            Context.Parent.Tell(destroyAsteroid);
            Context.Stop(Self);
            return true;
        }

        protected override void PostStop()
        {
            _asteroid.Tell(new View.Asteroid.Die());
            base.PostStop();
        }

        public class DestroyAsteroid
        {
        }
    }
}