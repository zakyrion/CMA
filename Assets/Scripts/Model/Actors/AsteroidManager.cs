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
    /*private readonly Random _random = new Random();
    private bool _isStart;

    public AsteroidManager() : base(new MainThreadController())
    {
    }

    protected override void Subscribe()
    {
        PushMessage<StartWithDificult>(OnStartWithDificult);
        PushMessage<CreateAsteroid>(OnCreateAsteroid);
        PushMessage<DestroyAsteroid>(OnDestroyAsteroid);
        PushMessage<Main.GameOver>(OnGameOver);
    }

    private void OnGameOver(IMessage message)
    {
        Debug.Log("Game Over");
        _isStart = false;
        Send(new Kill(), $"{Adress}/*");
    }

    private void OnStartWithDificult(StartWithDificult data, IMessage message)
    {
        Debug.Log("OnStartGameWithDificult");
        _isStart = true;
        var count = (int) data.Data;

        for (var i = 0; i < count; i++)
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
                _random.Next(700, 3000), true);
    }

    private void NeedToCreateAsteroid(object state, bool timedOut)
    {
        ThreadController.Invoke<IMessage>(OnCreateAsteroid, null);
    }

    private void OnDestroyAsteroid(DestroyAsteroid data, IMessage message)
    {
        Send(new Kill(), $"{Adress}/{data.Data}");

        ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), NeedToCreateAsteroid, null,
            _random.Next(200, 1000), true);
    }

    private void OnCreateAsteroid(IMessage message)
    {
        if (_isStart)
            Ask<Rect>(rect =>
            {
                if (_isStart)
                    Core.Get<Asteroid>(new BuildAsteroidMessage(rect));
            });
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
    }*/
}