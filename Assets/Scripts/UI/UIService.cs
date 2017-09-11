using CMA;
using Model;
using UnityEngine;

public class UIService : MonoActor<string>
{
    private Dificult _dificult;
    [SerializeField] private GameObject _dificultPanel;

    [SerializeField] private GameObject _gameOver;

    protected override void Awake()
    {
        Debug.Log("Init UI Service");
        Init("UIService");
        Main.Instance.AddActor(this);
    }

    private void OnShowGameOverUI()
    {
        _gameOver.SetActive(true);
        _dificultPanel.SetActive(false);
    }

    private void OnShowDificultUI()
    {
        Debug.Log("Catch: OnShowDificultUI");
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