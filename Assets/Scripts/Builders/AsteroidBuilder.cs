using Akka.Actor;
using UnityAkkaExtension;
using UnityEngine;
using Random = System.Random;

namespace Model
{
    public class AsteroidBuilder : Builder<View.Asteroid, BuildAsteroidMessage>
    {
        private readonly int _maxIndex = 5;
        private readonly string _path = "Prefabs/Asteroids/Asteroid_";
        private readonly Random _random = new Random();


        public override object Build()
        {
            return null;
        }

        public override object Build(BuildAsteroidMessage param)
        {
            var asteroid = Object.Instantiate(Resources.Load<GameObject>(_path + _random.Next(1, _maxIndex)));

            asteroid.transform.position = new Vector3(param.Borders.xMax + 1f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            var destination = new Vector3(param.Borders.xMin - 2f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            asteroid.GetComponent<View.Asteroid>()
                .Init(param.ActorRef, (float) (5f + 5f * _random.NextDouble()), destination);

            return asteroid;
        }
    }

    public class BuildAsteroidMessage
    {
        public BuildAsteroidMessage(IActorRef actorRef, Rect borders)
        {
            Borders = borders;
            ActorRef = actorRef;
        }

        public Rect Borders { get; protected set; }
        public IActorRef ActorRef { get; protected set; }
    }
}