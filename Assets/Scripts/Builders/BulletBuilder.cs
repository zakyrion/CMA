using System.Threading;
using Akka.Actor;
using UnityAkkaExtension;
using UnityEngine;

namespace Model
{
    public class BuildBulletMessage
    {
        public BuildBulletMessage(IActorRef actorRef, Vector3 position, Rect borders)
        {
            ActorRef = actorRef;
            Position = position;
            Borders = borders;
        }

        public Rect Borders { get; protected set; }
        public IActorRef ActorRef { get; protected set; }
        public Vector3 Position { get; protected set; }
    }

    public class BulletBuilder : Builder<View.Bullet, BuildBulletMessage>
    {
        private static int _index = int.MinValue;
        private readonly string _path = "Prefabs/Rocket";

        public static int GetIndex => Interlocked.Increment(ref _index);

        public override object Build()
        {
            return null;
        }

        public override object Build(BuildBulletMessage param)
        {
            var bullet = Object.Instantiate(Resources.Load<GameObject>(_path));
            bullet.GetComponent<View.Bullet>().Init(param.Position, param.ActorRef, param.Borders);
            return bullet;
        }
    }
}