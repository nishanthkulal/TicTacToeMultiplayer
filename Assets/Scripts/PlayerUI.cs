using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameobject;
    [SerializeField] private GameObject circleArrowGameobject;
    [SerializeField] private GameObject crossYouTextGameobject;
    [SerializeField] private GameObject circleYouTextGameobject;

    private void Awake()
    {
        crossArrowGameobject.SetActive(false);
        circleArrowGameobject.SetActive(false);
        crossYouTextGameobject.SetActive(false);
        circleYouTextGameobject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.CurrentPlayableplayerTypeChanged += GameManager_CurrentPlayableplayerTypeChanged;
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
