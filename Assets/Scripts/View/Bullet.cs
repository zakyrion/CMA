using CMA;
using UnityEngine;

namespace View
{
    public class Bullet : MonoActor<int>
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
                Send(new BulletManager.DestroyBullet(TypedKey));
            }
        }

        public void Init(int id, Rect borders)
        {
            Init(id);
            _borders = borders;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                Send(new BulletManager.DestroyBullet(TypedKey));
            }
        }

        private void OnDie()
        {
            Destroy(gameObject);
        }

        protected override void Subscribe()
        {
            Receive<Die>(OnDie);
        }

        public class Die
        {
        }
    }
}