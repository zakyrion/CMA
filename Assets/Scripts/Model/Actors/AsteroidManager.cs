using System.Threading;
using CMA;
using CMA.Core;
using CMA.Markers;
using CMA.Messages;
using Model;
using UnityEngine;
using View;
using Random = System.Random;

public class AsteroidManager : Actor<string>
{
    private readonly Random _random = new Random();
    private bool _isStart;

    public AsteroidManager() : base("AsteroidManager")
    {
    }

    public AsteroidManager(string key, IMessageManager manager) : base(key, manager)
    {
    }

    protected override void Subscribe()
    {
        AddMarker<AsteroidMarker>();

        Receive<StartWithDificult>(OnStartWithDificult);
        Receive<CreateAsteroid>(OnCreateAsteroid);
        Receive<DestroyAsteroid>(OnDestroyAsteroid);
        Receive<Main.GameOver>(OnGameOver);
    }

    private void OnGameOver(Main.GameOver message)
    {
        _isStart = false;
        var childs = Childs.ToArray();
        foreach (var child in childs)
        {
            child.Send(new Asteroid.Die());
            RemoveActor(child);
        }
    }

    private void OnStartWithDificult(StartWithDificult message)
    {
        _isStart = true;
        var count = (int) message.Data;

        for (var i = 0; i < count; i++)
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
                _random.Next(700, 3000), true);
    }

    private void NeedToCreateAsteroid(object state, bool timedOut)
    {
        Send(new CreateAsteroid());
    }

    private void OnDestroyAsteroid(DestroyAsteroid message)
    {
        var asteroid = GetActor<IActor, int>(message.Data);
        if (asteroid != null)
        {
            asteroid.Send(new Asteroid.Die());
            RemoveActor(asteroid);

            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
                _random.Next(200, 1000), true);
        }
    }

    private void OnCreateAsteroid(CreateAsteroid message)
    {
        if (_isStart)
        {
            var request = new SimpleRequest<Rect>(rect =>
            {
                Main.Instance.InvokeAt(() =>
                {
                    if (_isStart)
                    {
                        var asteroid = Core.Get<Asteroid>(new BuildAsteroidMessage(rect));
                        AddActor(asteroid);
                    }
                });
            });
            Send(request);
        }
    }

    public class AsteroidMarker : Marker<int>
    {
        public AsteroidMarker(int key) : base(key)
        {
        }
    }

    public class CreateAsteroid
    {
    }

    public class DestroyAsteroid : Container<int>
    {
        public DestroyAsteroid(int data) : base(data)
        {
        }
    }

    public class StartWithDificult : Container<Dificult>
    {
        public StartWithDificult(Dificult data) : base(data)
        {
        }
    }
}