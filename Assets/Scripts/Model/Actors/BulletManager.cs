using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

public class BulletManager : Actor
{
    private bool _isStart;

    protected override void Subscribe()
    {
        Receive<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
        Receive<Main.GameOver>(OnGameOver);
        Receive<CreateBullet>(OnCreateBullet);
    }

    private void OnStartGameWithDificult(IMessage message)
    {
        _isStart = true;
    }

    private void OnCreateBullet(CreateBullet data, IMessage message)
    {
        if (_isStart)
        {
            /*var request = new SimpleRequest<Rect>(rect =>
            {
                if (_isStart)
                    Main.Instance.InvokeAt(
                        () => { AddActor(Core.Get<Bullet>(new BuildBulletMessage(data.Data, rect))); });
            });

            Send(request);*/
        }
    }

    private void OnGameOver(IMessage message)
    {
        _isStart = false;
        /*var childs = Childs.ToArray();
        foreach (var child in childs)
        {
            child.Send(new Bullet.Die());
            RemoveActor(child);
        }*/
    }

    public class CreateBullet : Container<Vector3>
    {
        public CreateBullet(Vector3 data) : base(data)
        {
        }
    }
}