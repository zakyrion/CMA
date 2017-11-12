using System.Threading;
using CMA;
using CMA.Core;
using CMA.Messages;
using Model;
using UnityEngine;
using View;
using Random = System.Random;

public class AsteroidManager : Actor
{
    private readonly Random _random = new Random();
    private bool _isStart;

    protected override void Subscribe()
    {
        Receive<StartWithDificult>(OnStartWithDificult);
        Receive<CreateAsteroid>(OnCreateAsteroid);
        Receive<DestroyAsteroid>(OnDestroyAsteroid);
        Receive<Main.GameOver>(OnGameOver);
    }

    private void OnGameOver(IMessage message)
    {
        _isStart = false;
        /*var childs = Childs.ToArray();
        foreach (var child in childs)
        {
            child.Send(new Asteroid.Die());
            RemoveActor(child);
        }*/
    }

    private void OnStartWithDificult(StartWithDificult data, IMessage message)
    {
        _isStart = true;
        var count = (int) data.Data;

        for (var i = 0; i < count; i++)
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
                _random.Next(700, 3000), true);
    }

    private void NeedToCreateAsteroid(object state, bool timedOut)
    {
        Send(new CreateAsteroid());
    }

    private void OnDestroyAsteroid(DestroyAsteroid data, IMessage message)
    {
        /*var asteroid = GetActor<IActor, int>(data.Data);
        if (asteroid != null)
        {
            asteroid.Send(new Asteroid.Die());
            RemoveActor(asteroid);

            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
                _random.Next(200, 1000), true);
        }*/
    }

    private void OnCreateAsteroid(IMessage message)
    {
        /*if (_isStart)
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
        }*/
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