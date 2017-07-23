using CMA.Messages;
using Model;
using UnityEngine;

public class UIService : MonoBehaviour
{
    private Dificult _dificult;
    [SerializeField] private GameObject _dificultPanel;

    [SerializeField] private GameObject _gameOver;

    private void Awake()
    {
        Main.Instance.SubscribeMessage<ShowDificultUI>(OnShowDificultUI);
        Main.Instance.SubscribeMessage<ShowGameOver>(OnShowGameOverUI);
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
        Main.Instance.SendMessage(new AsteroidManager.StartWithDificult(_dificult));
    }

    public void Restart()
    {
        _gameOver.SetActive(false);
        Main.Instance.SendMessage(new AsteroidManager.StartWithDificult(_dificult));
    }

    public class ShowDificultUI : Message
    {
        public ShowDificultUI() : base(null)
        {
        }
    }

    public class ShowGameOver : Message
    {
        public ShowGameOver() : base(null)
        {
        }
    }
}