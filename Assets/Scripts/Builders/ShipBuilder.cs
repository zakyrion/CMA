using Akka.Actor;
using UnityAkkaExtension;
using UnityEngine;

namespace Model
{
    public class BuildShipMessage
    {
        public BuildShipMessage(IActorRef actorRef, Rect borders)
        {
            Borders = borders;
            ActorRef = actorRef;
        }

        public Rect Borders { get; protected set; }
        public IActorRef ActorRef { get; protected set; }
    }

    public class ShipBuilder : Builder<View.Ship, BuildShipMessage>
    {
        private readonly string _path = "Prefabs/SpaceshipFighter";

        public override object Build()
        {
            return null;
        }

        public override object Build(BuildShipMessage param)
        {
            var ship = Object.Instantiate(Resources.Load<GameObject>(_path));

            ship.transform.position = new Vector3(param.Borders.xMin + 1f, 0f, 0f);
            ship.GetComponent<View.Ship>().InitActor(param.ActorRef);

            return ship;
        }
    }
}