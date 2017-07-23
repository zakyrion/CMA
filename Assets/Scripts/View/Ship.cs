using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    public class Ship : MonoBehaviour
    {
        private Rect _border;
        [SerializeField] private float _cooldown;
        private bool _isSendDestroy;
        private Model.Ship _ship;
        [SerializeField] private float _speed;
        private float _timer;

        // Use this for initialization
        private void Start()
        {
            var requset = new SimpleRequest<Rect>(rect => { _border = rect; });
            Main.Instance.SendMessage(requset);
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
                Main.Instance.SendMessage(new BulletManager.CreateBullet(transform.position + Vector3.right));
            }

            var newZ = Mathf.Clamp(transform.position.z + dir * _speed * Time.deltaTime, _border.yMin + 1,
                _border.yMax - 1);
            transform.position = new Vector3(transform.position.x, 0f, newZ);
        }

        public void Init(Model.Ship ship)
        {
            _ship = ship;
            _ship.SubscribeMessage<Die>(OnDie);
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
                Main.Instance.SendMessage(new Main.GameOver());
            }
        }

        public class Die : Message
        {
            public Die() : base(null)
            {
            }
        }
    }
}