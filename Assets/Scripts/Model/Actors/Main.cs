using Akka.Actor;
using UnityAkkaExtension;
using UnityAkkaExtension.Messages;
using UnityEngine;

namespace Model
{
    public class Main : ReceiveActor
    {
        private Rect _rect;
        private UIService _uiService;

        public Main()
        {
            Core.SubscribeBuilder(new AsteroidBuilder());
            Core.SubscribeBuilder(new ShipBuilder());
            Core.SubscribeBuilder(new BulletBuilder());

            Context.ActorOf<AsteroidManager>("AsteroidManager");
            Context.ActorOf<BulletManager>("BulletManager");

            Receive<InitGame>(OnInitGame);
            Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
            Receive<GameOver>(OnGameOver);

            Receive<CreateAsteroid>(OnCreateAsteroid);
            Receive<CreateBullet>(OnCreateBullet);

            Receive<Rect>(rect =>
            {
                _rect = rect;
                Debug.Log("Catch Rect");
            });

            Receive<UIService>(service =>
            {
                _uiService = service;
                _uiService.InitActor(Self);
                Debug.Log("Catch UI");
            });

            Receive<SimpleRequest<Rect>>(request => request.Done(_rect));
        }

        public bool OnCreateAsteroid(CreateAsteroid message)
        {
            Debug.Log("Catch Create Asteroid");
            _uiService.InvokeAt(() => Core.Get<View.Asteroid>(new BuildAsteroidMessage(message.Data, _rect)));
            return true;
        }

        public bool OnCreateBullet(CreateBullet message)
        {
            Debug.Log("Catch Create Bullet");
            _uiService.InvokeAt(
                () => Core.Get<View.Bullet>(new BuildBulletMessage(message.Data, message.Position, _rect)));
            return true;
        }

        private bool OnGameOver(GameOver message)
        {
            _uiService.Tell(new UIService.ShowGameOver());

            var childs = Context.GetChildren();

            foreach (var child in childs)
                child.Tell(message);

            return true;
        }

        private bool OnStartGameWithDificult(AsteroidManager.StartWithDificult message)
        {
            var actor = Context.ActorOf<Ship>();
            _uiService.InvokeAt(() => Core.Get<View.Ship>(new BuildShipMessage(actor, _rect)));

            var childs = Context.GetChildren();
            foreach (var child in childs)
                child.Tell(message);

            return true;
        }

        private bool OnInitGame(InitGame message)
        {
            _uiService.Tell(new UIService.ShowDificultUI());
            return true;
        }

        public class InitGame
        {
        }

        public class GameOver
        {
        }

        public class CreateAsteroid : Message<IActorRef>
        {
            public CreateAsteroid(IActorRef data) : base(data)
            {
            }
        }

        public class CreateBullet : Message<IActorRef>
        {
            public CreateBullet(IActorRef data, Vector3 position) : base(data)
            {
                Position = position;
            }

            public Vector3 Position { get; protected set; }
        }
    }
}