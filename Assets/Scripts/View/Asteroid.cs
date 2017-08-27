using Akka.Actor;
using UnityAkkaExtension;
using UnityAkkaExtension.Messages;
using UnityEngine;

namespace View
{
    public class Asteroid : MonoActor
    {
        [SerializeField] private Vector3 _destination;

        private bool _isSendDestroy;
        [SerializeField] private float _speed;

        public void Init(IActorRef asteroid, float speed, Vector3 destination)
        {
            InitActor(asteroid);
            _speed = speed;
            _destination = destination;
            asteroid.Tell(this);
        }

        private void Update()
        {
            var diff = _destination - transform.position;
            if (diff.magnitude < .1f && !_isSendDestroy)
            {
                _isSendDestroy = true;
                ActorRef.Tell(new Model.Asteroid.DestroyAsteroid());
            }
            else
            {
                transform.position += diff.normalized * _speed * Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                ActorRef.Tell(new Model.Asteroid.DestroyAsteroid());
            }
        }

        private void OnDie(Die die)
        {
            Destroy(gameObject);
        }

        protected override void Subscribe()
        {
            Receive<Transform>(message => message.Done(transform));
            Receive<Die>(OnDie);
        }

        public class Die : Message
        {
        }
    }
}