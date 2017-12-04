using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;
using View;

namespace Model
{
    public class Main : Actor
    {
        /*static Main()
        {
            Core.SubscribeBuilder(new AsteroidBuilder());
            Core.SubscribeBuilder(new ShipBuilder());
            Core.SubscribeBuilder(new BulletBuilder());

            var actor = new Main();
            
            Instance = new MailBox(new Adress("Main"), new ActorSystem());

            Instance.AddActor(actor);

            var asteroidManager = new AsteroidManager();
            Instance.AddActor(asteroidManager, "Main/AsteroidManager");

            var bulletManager = new BulletManager();
            Instance.AddActor(bulletManager, "Main/BulletManager");
        }

        public Main() : base(new MainThreadController())
        {
        }

        public static MailBox Instance { get; set; }

        protected override void Subscribe()
        {
            PushMessage<InitGame>(OnInitGame);
            PushMessage<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
            PushMessage<GameOver>(OnGameOver);
        }

        private void OnGameOver(IMessage message)
        {
            Send(new UIService.ShowGameOver());
        }

        private void OnStartGameWithDificult(IMessage message)
        {
            Debug.Log("OnStartGameWithDificult");

            Ask<Rect>(rect =>
            {
                Debug.Log("Catch Rect Responce");
                Core.Get<Ship>(new BuildShipMessage(rect));
            }, "Main/BorderCalculator");
        }

        private void OnInitGame(IMessage message)
        {
            Debug.Log("Catch: OnInitGame");
            Send(new UIService.ShowDificultUI());
        }

        public class InitGame
        {
        }

        public class GameOver
        {
        }*/
    }
}