using System.Threading;
using CMA;
using UnityEngine;
using View;
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
            return asteroidGo.GetComponent<Asteroid>();
        }

        public override object Build(BuildAsteroidMessage param)
        {
            var asteroidGo = Object.Instantiate(Resources.Load<GameObject>(_path + _random.Next(1, _maxIndex)));
            var asteroid = asteroidGo.GetComponent<Asteroid>();

            asteroidGo.transform.position = new Vector3(param.Borders.xMax + 1f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            var destination = new Vector3(param.Borders.xMin - 2f, 0f,
                param.Borders.yMin + (param.Borders.yMax - param.Borders.yMin) * (float) _random.NextDouble());

            //asteroid.Init(GetIndex, (float) (5f + 5f * _random.NextDouble()), destination);

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