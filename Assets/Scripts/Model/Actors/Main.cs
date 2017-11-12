using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

namespace Model
{
    public class Main : Actor
    {
        static Main()
        {
            Core.SubscribeBuilder(new AsteroidBuilder());
            Core.SubscribeBuilder(new ShipBuilder());
            Core.SubscribeBuilder(new BulletBuilder());

            var actor = new Main();

            Instance = new MailBox(new Adress("Main"));
            Instance.AddActor(actor);

            var asteroidManager = new AsteroidManager();
            Instance.AddActor(asteroidManager, "Main/AsteroidManager");

            var bulletManager = new BulletManager();
            Instance.AddActor(bulletManager, "Main/BulletManager");
        }

        public static MailBox Instance { get; set; }

        protected override void Subscribe()
        {
            Receive<InitGame>(OnInitGame);
            Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
            Receive<GameOver>(OnGameOver);
        }

        private void OnGameOver(IMessage message)
        {
            Send(new UIService.ShowGameOver());

            /*foreach (var child in Childs)
                child.Send(Message);*/
        }

        private void OnStartGameWithDificult(IMessage message)
        {
            Debug.Log("OnStartGameWithDificult");
            /*var request = new SimpleRequest<Rect>(rect =>
            {
                Debug.Log("Catch Rect Responce");
                AddActor(Core.Get<Ship>(new BuildShipMessage(rect)));
            });
            Send(request);*/

            /*foreach (var child in Childs)
                child.Send(Message);*/
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
        }
    }
}