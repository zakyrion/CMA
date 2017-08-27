using Akka.Actor;
using UnityEngine;

namespace Model
{
    public class Ship : ReceiveActor
    {
        private View.Ship _ship;

        public Ship()
        {
            Receive<Main.GameOver>(OnGameOver);
            Receive<View.Ship>(ship =>
            {
                _ship = ship;
                Debug.Log("Catch Ship");
            });
        }

        private bool OnGameOver(Main.GameOver message)
        {
            _ship.Tell(new View.Ship.Die());
            Context.Stop(Self);
            return true;
        }

        public class DestroyShip
        {
        }
    }
}