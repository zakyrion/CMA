using CMA;
using UnityEngine;

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
            var ship = new Ship();

            shipGO.transform.position = new Vector3(param.Borders.xMin + 1f, 0f, 0f);
            shipGO.GetComponent<View.Ship>().Init(ship);

            return ship;
        }
    }
}