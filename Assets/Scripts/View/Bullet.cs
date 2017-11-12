using CMA;
using CMA.Messages;
using Model;
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
                Send(new Kill(), Adress);
            }
        }

        public void Init(int id, Rect borders)
        {
            Main.Instance.AddActor(this, $"Main/BulletManager/{id}");
            _borders = borders;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                Send(new Kill(), Adress);
            }
        }

        protected override void Subscribe()
        {
        }
    }
}