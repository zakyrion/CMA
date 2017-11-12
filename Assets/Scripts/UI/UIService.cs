using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

public class UIService : MonoActor
{
    private Dificult _dificult;
    [SerializeField] private GameObject _dificultPanel;

    [SerializeField] private GameObject _gameOver;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Init UI Service");
        Main.Instance.AddActor(this, "Main/UIService");
    }

    private void OnShowGameOverUI(IMessage message)
    {
        _gameOver.SetActive(true);
        _dificultPanel.SetActive(false);
    }

    private void OnShowDificultUI(IMessage message)
    {
        Debug.Log("Catch: OnShowDificultUI");
        _gameOver.SetActive(false);
        _dificultPanel.SetActive(true);
    }

    public void StartGame(int dificult)
    {
        _dificult = (Dificult) dificult;
        _dificultPanel.SetActive(false);
        Send(new AsteroidManager.StartWithDificult(_dificult));
    }

    public void Restart()
    {
        _gameOver.SetActive(false);
        Send(new AsteroidManager.StartWithDificult(_dificult));
    }

    protected override void Subscribe()
    {
        Receive<ShowDificultUI>(OnShowDificultUI);
        Receive<ShowGameOver>(OnShowGameOverUI);
    }

    public class ShowDificultUI
    {
    }

    public class ShowGameOver
    {
    }
}