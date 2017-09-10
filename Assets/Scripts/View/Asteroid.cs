using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Asteroid : MonoActor<int>
    {
        [SerializeField] private Vector3 _destination;

        private bool _isSendDestroy;
        [SerializeField] private float _speed;

        public void Init(int id, float speed, Vector3 destination)
        {
            Init(id);
            _speed = speed;
            _destination = destination;

            /*_asteroid = asteroid;
            _asteroid.Receive<Transform>( ()=> Message.Done(transform));
            _asteroid.Receive<Die>(OnDie);*/
        }

        private void Update()
        {
            var diff = _destination - transform.position;
            if (diff.magnitude < .1f && !_isSendDestroy)
            {
                _isSendDestroy = true;
                //Main.Instance.Send(new AsteroidManager.DestroyAsteroid(_asteroid.TypedKey));
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
                //Main.Instance.Send(new AsteroidManager.DestroyAsteroid(_asteroid.TypedKey));
            }
        }

        private void OnDie(Die die)
        {
            Main.Instance.InvokeAt(() => Destroy(gameObject));
        }

        public class Die : Message
        {
            public Die() : base(null)
            {
            }
        }

        protected override void Subscribe()
        {
            
        }
    }
}