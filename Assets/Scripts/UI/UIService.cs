using CMA;
using Model;
using UnityEngine;

public class UIService : MonoActor<string>
{
    private Dificult _dificult;
    [SerializeField] private GameObject _dificultPanel;

    [SerializeField] private GameObject _gameOver;

    private void Awake()
    {
        Init("UIService");
        Main.Instance.AddActor(this);
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
        Main.Instance.Send(new AsteroidManager.StartWithDificult(_dificult));
    }

    public void Restart()
    {
        _gameOver.SetActive(false);
        Main.Instance.Send(new AsteroidManager.StartWithDificult(_dificult));
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