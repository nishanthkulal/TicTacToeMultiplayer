using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform playSoundeffectPrefab;
    [SerializeField] private Transform playerwinSoundeffectPrefab;
    [SerializeField] private Transform playerlooseSoundeffectPrefab;
    private void Start()
    {
        GameManager.Instance.OnObjectPlaced += Instance_OnObjectPlaced;
        GameManager.Instance.OnGameWin += Instance_OnGameWin;
    }

    private void Instance_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (GameManager.Instance.GetLocalPlayerType() == e.winPlayerType)
        {
            Transform winsoundTrnasform = Instantiate(playerwinSoundeffectPrefab);
            Destroy(winsoundTrnasform.gameObject, 5f);
        }
        else
        {
            Transform loosesoundTrnasform = Instantiate(playerlooseSoundeffectPrefab);
            Destroy(loosesoundTrnasform.gameObject, 5f);
        }
    }

    private void Instance_OnObjectPlaced(object sender, EventArgs e)
    {
        Transform playSoundTrnasform = Instantiate(playSoundeffectPrefab);
        Destroy(playSoundTrnasform.gameObject, 5f);
    }
}
