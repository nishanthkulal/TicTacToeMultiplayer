using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tieColor;
    [SerializeField] private Button reMatchButton;

    private void Awake()
    {
        reMatchButton.onClick.AddListener(() => { GameManager.Instance.ReMatchRpc(); });
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnReMatch += GameManager_OnReMatch;
        GameManager.Instance.OnGameTied += GameManager_OnGameTied;
        Hide();
    }

    private void GameManager_OnGameTied(object sender, EventArgs e)
    {
        Show();
        resultText.text = "TIE!";
        resultText.color = tieColor;
    }

    private void GameManager_OnReMatch(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            resultText.text = "You Win!";
            resultText.color = winColor;
        }
        else
        {
            resultText.text = "You Lose!";
            resultText.color = loseColor;
        }
        Show();

    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

