using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Bullet : MonoBehaviour
    {
        private Rect _borders;
        private Model.Bullet _bullet;
        private bool _isSendDestroy;
        [SerializeField] private float _speed;

        // Update is called once per frame
        private void Update()
        {
            transform.position += Vector3.right * _speed * Time.deltaTime;
            if (!_isSendDestroy && transform.position.x > _borders.xMax + 1)
            {
                _isSendDestroy = true;
                Main.Instance.SendMessage(new BulletManager.DestroyBullet(_bullet.TypedKey));
            }
        }

        public void Init(Model.Bullet bullet, Rect borders)
        {
            _bullet = bullet;
            _borders = borders;

            _bullet.SubscribeMessage<Die>(OnDie);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                Main.Instance.SendMessage(new BulletManager.DestroyBullet(_bullet.TypedKey));
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