using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;
using View;

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
            Receive<InitGame>(OnInitGame);
            Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
            Receive<GameOver>(OnGameOver);
        }

        private void OnGameOver()
        {
            Send(new UIService.ShowGameOver());

            foreach (var child in Childs)
                child.Send(Message);
        }

        private void OnStartGameWithDificult()
        {
            Debug.Log("OnStartGameWithDificult");
            var request = new SimpleRequest<Rect>(rect =>
            {
                Debug.Log("Catch Rect Responce");
                AddActor(Core.Get<Ship>(new BuildShipMessage(rect)));
            });
            Send(request);

            foreach (var child in Childs)
                child.Send(Message);
        }

        private void OnInitGame(InitGame message)
        {
            Debug.Log("Catch: OnInitGame");
            Send(new UIService.ShowDificultUI());
        }

        public class InitGame
        {
        }

        public class GameOver
        {
        }
    }
}