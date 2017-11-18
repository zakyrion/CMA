using CMA;
using Model;
using UnityEngine;

namespace View
{
    public class Ship : MonoActor
    {
        private Rect _border;
        [SerializeField] private float _cooldown;
        private bool _isSendDestroy;
        [SerializeField] private float _speed;
        private float _timer;

        protected override void Awake()
        {
            base.Awake();
            Main.Instance.AddActor(this, "Main/Ship");
        }

        public override void OnAdd(IMailBox mailBox)
        {
            base.OnAdd(mailBox);

            Debug.Log("Ask rect");
            Ask<Rect>(rect =>
            {
                Debug.Log("Catch Rect Responce");
                _border = rect;
            });
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
                Send(new BulletManager.CreateBullet(transform.position + Vector3.right));
            }

            var newZ = Mathf.Clamp(transform.position.z + dir * _speed * Time.deltaTime, _border.yMin + 1,
                _border.yMax - 1);
            transform.position = new Vector3(transform.position.x, 0f, newZ);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                Send(new Main.GameOver());
                Destroy(gameObject);
            }
        }

        protected override void Subscribe()
        {
        }
    }
}