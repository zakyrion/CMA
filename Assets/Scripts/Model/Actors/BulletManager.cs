using CMA;
using CMA.Core;
using CMA.Messages;
using Model;
using UnityEngine;
using View;

public class BulletManager : Actor
{
    /*private bool _isStart;

    public BulletManager() : base(new MainThreadController())
    {
    }

    protected override void Subscribe()
    {
        PushMessage<AsteroidManager.StartWithDificult>(OnStartGameWithDificult);
        PushMessage<Main.GameOver>(OnGameOver);
        PushMessage<CreateBullet>(OnCreateBullet);
    }

    private void OnStartGameWithDificult(IMessage message)
    {
        Debug.Log("OnStartGameWithDificult");
        _isStart = true;
    }

    private void OnCreateBullet(CreateBullet data, IMessage message)
    {
        if (_isStart)
            Ask<Rect>(rect =>
            {
                if (_isStart)
                    Core.Get<Bullet>(new BuildBulletMessage(data.Data, rect));
            },"Main/BorderCalculator");
    }

    private void OnGameOver(IMessage message)
    {
        _isStart = false;
        Send(new Kill(), $"{Adress}/*");
    }

    public class CreateBullet : Container<Vector3>
    {
        public CreateBullet(Vector3 data) : base(data)
        {
        }
    }*/
}