using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Asteroid : MonoBehaviour
    {
        private Model.Asteroid _asteroid;

        [SerializeField] private Vector3 _destination;

        private bool _isSendDestroy;
        [SerializeField] private float _speed;

        public void Init(Model.Asteroid asteroid, float speed, Vector3 destination)
        {
            _speed = speed;
            _destination = destination;

            _asteroid = asteroid;
            _asteroid.SubscribeMessage<Transform>(message => message.Done(transform));
            _asteroid.SubscribeMessage<Die>(OnDie);
        }

        private void Update()
        {
            var diff = _destination - transform.position;
            if (diff.magnitude < .1f && !_isSendDestroy)
            {
                _isSendDestroy = true;
                Main.Instance.SendMessage(new AsteroidManager.DestroyAsteroid(_asteroid.TypedKey));
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
                Main.Instance.SendMessage(new AsteroidManager.DestroyAsteroid(_asteroid.TypedKey));
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
    }
}