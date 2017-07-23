using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace Model
{
    public class Main : Actor<string>
    {
        static Main()
        {
            Core.SubscribeBuilder(new AsteroidBuilder());
            Core.SubscribeBuilder(new ShipBuilder());
            Core.SubscribeBuilder(new BulletBuilder());
            
            Instance = new Main();
            Instance.AddActor(new AsteroidManager());
            Instance.AddActor(new BulletManager());
        }

        public Main() : base("Main", new UpdatedMessageManager())
        {
        }

        public Main(string key, IMessageManager manager) : base(key, manager)
        {
        }

        public static Main Instance { get; set; }

        protected override void Subscribe()
        {
            SubscribeMessage<InitGame>(OnInitGame);
            SubscribeMessage<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
            SubscribeMessage<GameOver>(OnGameOver);
        }

        private void OnGameOver(GameOver message)
        {
            SendMessage(new UIService.ShowGameOver());

            foreach (var child in Childs)
                child.SendMessage(message);
        }

        private void OnStartGameWithDificult(AsteroidManager.StartWithDificult message)
        {
            var request = new SimpleRequest<Rect>(rect => { AddActor(Core.Get<Ship>(new BuildShipMessage(rect))); });
            SendMessage(request);

            foreach (var child in Childs)
                child.SendMessage(message);
        }

        private void OnInitGame(InitGame message)
        {
            SendMessage(new UIService.ShowDificultUI());
        }

        public class InitGame : Message
        {
            public InitGame(IActionHandler action) : base(action)
            {
            }
        }

        public class GameOver : Message
        {
            public GameOver() : base(null)
            {
            }
        }
    }
}