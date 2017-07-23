using System.Threading;
using CMA;
using CMA.Messages;
using UnityEngine;
using Random = System.Random;

namespace Model
{
    public class AsteroidBuilder : Builder<Asteroid, BuildAsteroidMessage>
    {
        private static int _index = int.MinValue;
        private readonly int _maxIndex = 5;
        private readonly string _path = "Prefabs/Asteroids/Asteroid_";

        private readonly Random _random = new Random();

        public static int GetIndex
        {
            get { return Interlocked.Increment(ref _index); }
        }

        public override object Build()
        {
            var asteroidGo = Object.Instantiate(Resources.Load<GameObject>(_path + _random.Next(1, _maxIndex)));
            var asteroid = new Asteroid(GetIndex);
            return asteroid;
        }

        public override object Build(BuildAsteroidMessage param)
        {
            var asteroidGo = Object.Instantiate(Resources.Load<GameObject>(_path + _random.Next(1, _maxIndex)));
            var asteroid = new Asteroid(GetIndex);

            asteroidGo.transform.position = new Vector3(param.Borders.xMax + 1f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            var destination = new Vector3(param.Borders.xMin - 2f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            asteroidGo.GetComponent<View.Asteroid>()
                .Init(asteroid, (float) (5f + 5f * _random.NextDouble()), destination);

            return asteroid;
        }
    }

    public class BuildAsteroidMessage
    {
        public BuildAsteroidMessage(Rect borders)
        {
            Borders = borders;
        }

        public Rect Borders { get; protected set; }
    }
}