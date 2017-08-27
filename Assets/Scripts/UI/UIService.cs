using Model;
using UnityAkkaExtension;
using UnityAkkaExtension.Messages;
using UnityEngine;
using View;

public class UIService : MonoActor
{
    private Dificult _dificult;
    [SerializeField] private GameObject _dificultPanel;
    [SerializeField] private GameObject _gameOver;

    protected override void Awake()
    {
        base.Awake();
        StarGameManager.Context.ActorSelection(StarGameManager.Path + "*").Tell(this);
    }

    protected override void Subscribe()
    {
        Receive<ShowDificultUI>(OnShowDificultUI);
        Receive<ShowGameOver>(OnShowGameOverUI);
    }

    private void OnShowGameOverUI(ShowGameOver message)
    {
        _gameOver.SetActive(true);
        _dificultPanel.SetActive(false);
    }

    private void OnShowDificultUI(ShowDificultUI message)
    {
        _gameOver.SetActive(false);
        _dificultPanel.SetActive(true);
    }

    public void StartGame(int dificult)
    {
        _dificult = (Dificult) dificult;
        _dificultPanel.SetActive(false);
        ActorRef.Tell(new AsteroidManager.StartWithDificult(_dificult), null);
    }

    public void Restart()
    {
        _gameOver.SetActive(false);
        ActorRef.Tell(new AsteroidManager.StartWithDificult(_dificult), null);
    }

    public class ShowDificultUI : Message
    {
    }

    public class ShowGameOver : Message
    {
    }
}