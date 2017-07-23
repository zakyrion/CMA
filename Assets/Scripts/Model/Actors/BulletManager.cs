using CMA;
using CMA.Core;
using CMA.Messages;
using Model;
using UnityEngine;
using Bullet = View.Bullet;

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
        SubscribeMessage<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
        SubscribeMessage<Main.GameOver>(OnGameOver);
        SubscribeMessage<CreateBullet>(OnCreateBullet);
        SubscribeMessage<DestroyBullet>(OnDestroyBullet);
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
            bullet.SendMessage(new Bullet.Die());
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
                        () => { AddActor(Core.Get<Model.Bullet>(new BuildBulletMessage(message.Data, rect))); });
            });

            SendMessage(request);
        }
    }

    private void OnGameOver(Main.GameOver message)
    {
        _isStart = false;
        var childs = Childs.ToArray();
        foreach (var child in childs)
        {
            child.SendMessage(new Bullet.Die());
            RemoveActor(child);
        }
    }

    public class CreateBullet : Message<Vector3>
    {
        public CreateBullet(Vector3 data) : base(data)
        {
        }
    }

    public class DestroyBullet : Message<int>
    {
        public DestroyBullet(int data) : base(data)
        {
        }
    }
}