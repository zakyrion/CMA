using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Asteroid : MonoActor
    {
        [SerializeField] private Vector3 _destination;

        private bool _isSendDestroy;
        [SerializeField] private float _speed;
        private int _id;
        public void Init(int id, float speed, Vector3 destination)
        {
            _id = id;
            _speed = speed;
            _destination = destination;
            Main.Instance.AddActor(this, $"Main/AsteroidManager/{id}");
        }

        private void Update()
        {
            var diff = _destination - transform.position;
            if (diff.magnitude < .1f && !_isSendDestroy)
            {
                _isSendDestroy = true;
                Send(new AsteroidManager.DestroyAsteroid(_id), @"Main/AsteroidManager");
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
                Send(new AsteroidManager.DestroyAsteroid(_id), @"Main/AsteroidManager");
            }
        }

        protected override void Subscribe()
        {
            Receive<Transform>(OnTransformRequest);
        }

        private void OnTransformRequest(IMessage message)
        {
            Respounce(message, transform);
        }
    }
}