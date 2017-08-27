using Akka.Actor;
using UnityAkkaExtension;
using UnityAkkaExtension.Messages;
using UnityEngine;

namespace View
{
    public class Bullet : MonoActor
    {
        private Rect _borders;
        private bool _isSendDestroy;
        [SerializeField] private float _speed;

        // Update is called once per frame
        private void Update()
        {
            transform.position += Vector3.right * _speed * Time.deltaTime;
            if (!_isSendDestroy && transform.position.x > _borders.xMax + 1)
            {
                _isSendDestroy = true;
                ActorRef.Tell(new Model.Bullet.DestroyBullet());
            }
        }

        public void Init(Vector3 position, IActorRef bullet, Rect borders)
        {
            transform.position = position;
            InitActor(bullet);
            _borders = borders;
            ActorRef.Tell(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                ActorRef.Tell(new Model.Bullet.DestroyBullet());
            }
        }

        private void OnDie(Die die)
        {
            Destroy(gameObject);
        }

        protected override void Subscribe()
        {
            Receive<Die>(OnDie);
        }

        public class Die : Message
        {
        }
    }
}