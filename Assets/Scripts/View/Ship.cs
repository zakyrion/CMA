using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Ship : MonoActor<string>
    {
        private Rect _border;
        [SerializeField] private float _cooldown;
        private bool _isSendDestroy;
        [SerializeField] private float _speed;
        private float _timer;

        // Use this for initialization
        private void Start()
        {
            var requset = new SimpleRequest<Rect>(rect => { _border = rect; });
            Main.Instance.Send(requset);
        }

        // Update is called once per frame
        private void Update()
        {
            var dir = 0;

            if (Input.GetKey(KeyCode.S))
                dir--;

            if (Input.GetKey(KeyCode.W))
                dir++;

            _timer += Time.deltaTime;

            if (Input.GetKey(KeyCode.Space) && _timer > _cooldown)
            {
                _timer = 0;
                Main.Instance.Send(new BulletManager.CreateBullet(transform.position + Vector3.right));
            }

            var newZ = Mathf.Clamp(transform.position.z + dir * _speed * Time.deltaTime, _border.yMin + 1,
                _border.yMax - 1);
            transform.position = new Vector3(transform.position.x, 0f, newZ);
        }

        private void OnDie(Die message)
        {
            Main.Instance.InvokeAt(() => { Destroy(gameObject); });
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                Main.Instance.Send(new Main.GameOver());
            }
        }

        protected override void Subscribe()
        {
            Receive<Die>(OnDie);
        }

        public class Die
        {
        }

        public class ShipId
        {
            public ShipId(int key)
            {
                Key = key;
            }

            public int Key { get; protected set; }

            public override bool Equals(object obj)
            {
                var keyObj = obj as ShipId;

                if (keyObj != null)
                    return keyObj.Key == Key;

                return base.Equals(obj);
            }
        }
    }
}