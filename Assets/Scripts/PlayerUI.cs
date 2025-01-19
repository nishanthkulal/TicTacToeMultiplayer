using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameobject;
    [SerializeField] private GameObject circleArrowGameobject;
    [SerializeField] private GameObject crossYouTextGameobject;
    [SerializeField] private GameObject circleYouTextGameobject;
    [SerializeField] private TextMeshProUGUI crossPlayerScoreText;
    [SerializeField] private TextMeshProUGUI circlePlayerScoreText;

    private void Awake()
    {
        crossArrowGameobject.SetActive(false);
        circleArrowGameobject.SetActive(false);
        crossYouTextGameobject.SetActive(false);
        circleYouTextGameobject.SetActive(false);
        crossPlayerScoreText.text = "";
        circlePlayerScoreText.text = "";
    }

    private void Start()
    {

        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.CurrentPlayableplayerTypeChanged += GameManager_CurrentPlayableplayerTypeChanged;
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        GameManager.Instance.GetScores(out int circleScore, out int crossScore);
        crossPlayerScoreText.text = crossScore.ToString();
        circlePlayerScoreText.text = circleScore.ToString();
    }


    private void GameManager_CurrentPlayableplayerTypeChanged(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }


    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextGameobject.SetActive(true);
        }
        else
        {
            circleYouTextGameobject.SetActive(true);
        }
        crossPlayerScoreText.text = "0";
        circlePlayerScoreText.text = "0";
        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            crossArrowGameobject.SetActive(true);
            circleArrowGameobject.SetActive(false);
        }
        else
        {
            crossArrowGameobject.SetActive(false);
            circleArrowGameobject.SetActive(true);
        }

    }

}
