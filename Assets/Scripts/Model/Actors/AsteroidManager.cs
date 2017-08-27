using System;
using Akka.Actor;
using Model;

public class AsteroidManager : ReceiveActor
{
    private readonly Random _random = new Random();
    private bool _isStart;

    public AsteroidManager()
    {
        Receive<StartWithDificult>(OnStartWithDificult);
        Receive<CreateAsteroid>(OnCreateAsteroid);
        Receive<Asteroid.DestroyAsteroid>(OnDestroyAsteroid);
        Receive<Main.GameOver>(OnGameOver);
    }

    private bool OnGameOver(Main.GameOver message)
    {
        _isStart = false;
        var childs = Context.GetChildren();

        foreach (var child in childs)
            child.Tell(PoisonPill.Instance, Self);

        return true;
    }

    private bool OnStartWithDificult(StartWithDificult message)
    {
        _isStart = true;
        var count = (int) message.Data;

        for (var i = 0; i < count; i++)
            Context.System.Scheduler.ScheduleTellOnce(_random.Next(700, 3000), Self, new CreateAsteroid(), null);

        return true;
    }

    private bool OnDestroyAsteroid(Asteroid.DestroyAsteroid message)
    {
        Context.System.Scheduler.ScheduleTellOnce(_random.Next(200, 1000), Self, new CreateAsteroid(), null);
        return true;
    }

    private bool OnCreateAsteroid(CreateAsteroid message)
    {
        if (_isStart)
            Context.Parent.Tell(new Main.CreateAsteroid(Context.ActorOf<Asteroid>()));

        return true;
    }

    public class CreateAsteroid
    {
    }

    public class StartWithDificult
    {
        public StartWithDificult(Dificult data)
        {
            Data = data;
        }

        public Dificult Data { get; protected set; }
    }
}