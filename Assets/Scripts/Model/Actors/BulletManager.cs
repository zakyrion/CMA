using CMA;
using CMA.Core;
using CMA.Messages;
using Model;
using UnityEngine;
using View;

public class BulletManager : Actor<string>
{
    private bool _isStart;

    public BulletManager() : base("BulletManager")
    {
    }

    public BulletManager(string key, IMessageManager manager) : base(key, manager)
    {
    }

    protected override void Subscribe()
    {
        Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
        Receive<Main.GameOver>(OnGameOver);
        Receive<CreateBullet>(OnCreateBullet);
        Receive<DestroyBullet>(OnDestroyBullet);
    }

    private void OnStartGameWithDificult(AsteroidManager.StartWithDificult message)
    {
        _isStart = true;
    }

    private void OnDestroyBullet(DestroyBullet message)
    {
        var bullet = GetActor<IActor, int>(message.Data);
        if (bullet != null)
        {
            bullet.Send(new Bullet.Die());
            RemoveActor(bullet);
        }
    }

    private void OnCreateBullet(CreateBullet message)
    {
        if (_isStart)
        {
            var request = new SimpleRequest<Rect>(rect =>
            {
                if (_isStart)
                    Main.Instance.InvokeAt(
                        () => { AddActor(Core.Get<Bullet>(new BuildBulletMessage(message.Data, rect))); });
            });

            Send(request);
        }
    }

    private void OnGameOver()
    {
        _isStart = false;
        var childs = Childs.ToArray();
        foreach (var child in childs)
        {
            child.Send(new Bullet.Die());
            RemoveActor(child);
        }
    }

    public class CreateBullet : Container<Vector3>
    {
        public CreateBullet(Vector3 data) : base(data)
        {
        }
    }

    public class DestroyBullet : Container<int>
    {
        public DestroyBullet(int data) : base(data)
        {
        }
    }
}