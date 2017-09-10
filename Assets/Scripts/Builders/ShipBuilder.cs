using CMA;
using UnityEngine;
using View;

namespace Model
{
    public class BuildShipMessage
    {
        public BuildShipMessage(Rect borders)
        {
            Borders = borders;
        }

        public Rect Borders { get; protected set; }
    }

    public class ShipBuilder : Builder<Ship, BuildShipMessage>
    {
        private readonly string _path = "Prefabs/SpaceshipFighter";

        public override object Build()
        {
            return null;
        }

        public override object Build(BuildShipMessage param)
        {
            var shipGO = Object.Instantiate(Resources.Load<GameObject>(_path));

            shipGO.transform.position = new Vector3(param.Borders.xMin + 1f, 0f, 0f);
            var ship = shipGO.GetComponent<Ship>();
            ship.Init("Ship");

            return ship;
        }
    }
}