using Akka.Actor;
using Model;
using UnityAkkaExtension;
using UnityAkkaExtension.Messages;
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

        // Use this for initialization
        private void Start()
        {
            var requset = new SimpleRequest<Rect>(this, rect => { _border = rect; });
            StarGameManager.Context.ActorSelection(StarGameManager.Path + "*").Tell(requset);
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
                Debug.Log("Try shoot");
                _timer = 0;
                StarGameManager.Context.ActorSelection(StarGameManager.Path + "Main/BulletManager")
                    .Tell(new BulletManager.CreateBullet(transform.position));
            }

            var newZ = Mathf.Clamp(transform.position.z + dir * _speed * Time.deltaTime, _border.yMin + 1,
                _border.yMax - 1);
            transform.position = new Vector3(transform.position.x, 0f, newZ);
        }

        public override void InitActor(IActorRef actorRef)
        {
            base.InitActor(actorRef);
            actorRef.Tell(this);
        }

        private void OnDie(Die message)
        {
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isSendDestroy)
            {
                _isSendDestroy = true;
                StarGameManager.Context.ActorSelection(StarGameManager.Path + "*").Tell(new Main.GameOver());
            }
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