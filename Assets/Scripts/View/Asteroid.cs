using CMA;
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
        }

        private void Update()
        {
            var diff = _destination - transform.position;
            if (diff.magnitude < .1f && !_isSendDestroy)
            {
                _isSendDestroy = true;
                Send(new AsteroidManager.DestroyAsteroid(TypedKey));
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
                Send(new AsteroidManager.DestroyAsteroid(TypedKey));
            }
        }

        private void OnDie()
        {
            Destroy(gameObject);
        }

        protected override void Subscribe()
        {
            Receive<Transform>(() => Message.Done(transform));
            Receive<Die>(OnDie);
        }

        public class Die
        {
        }
    }
}