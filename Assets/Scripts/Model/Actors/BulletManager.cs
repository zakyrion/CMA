using Akka.Actor;
using Model;
using UnityAkkaExtension.Messages;
using UnityEngine;

public class BulletManager : ReceiveActor
{
    private bool _isStart;

    public BulletManager()
    {
        Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
        Receive<Main.GameOver>(OnGameOver);
        Receive<CreateBullet>(OnCreateBullet);
    }

    private bool OnStartGameWithDificult(AsteroidManager.StartWithDificult message)
    {
        _isStart = true;
        return true;
    }

    private bool OnCreateBullet(CreateBullet message)
    {
        if (_isStart)
        {
            Debug.Log("Create Bullet");
            var actor = Context.ActorOf<Bullet>();
            Context.Parent.Tell(new Main.CreateBullet(actor, message.Data));
        }

        return true;
    }

    private bool OnGameOver(Main.GameOver message)
    {
        _isStart = false;
        var childs = Context.GetChildren();

        foreach (var child in childs)
            child.Tell(new Bullet.DestroyBullet());

        return true;
    }

    public class CreateBullet : Message<Vector3>
    {
        public CreateBullet(Vector3 data) : base(data)
        {
        }
    }
}