using System.Threading;
using CMA;
using UnityEngine;

namespace Model
{
    public class BuildBulletMessage
    {
        public BuildBulletMessage(Vector3 position, Rect borders)
        {
            Position = position;
            Borders = borders;
        }

        public Rect Borders { get; protected set; }
        public Vector3 Position { get; protected set; }
    }

    public class BulletBuilder : Builder<Bullet, BuildBulletMessage>
    {
        private static int _index = int.MinValue;
        private readonly string _path = "Prefabs/Rocket";

        public static int GetIndex
        {
            get { return Interlocked.Increment(ref _index); }
        }

        public override object Build()
        {
            return null;
        }

        public override object Build(BuildBulletMessage param)
        {
            var rocketGO = Object.Instantiate(Resources.Load<GameObject>(_path));
            var bullet = new Bullet(GetIndex);

            rocketGO.transform.position = param.Position;
            rocketGO.GetComponent<View.Bullet>().Init(bullet, param.Borders);

            return bullet;
        }
    }
}