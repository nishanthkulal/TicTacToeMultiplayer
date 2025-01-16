using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;
    private const float GRID_SIZE = 3.1f;
    private List<GameObject> visualGameObjectsList;
    private void Awake()
    {
        visualGameObjectsList = new List<GameObject>();
    }

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnReMatch += GameManager_OnReMatch;
    }

    private void GameManager_OnReMatch(object sender, EventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        foreach (GameObject visualGameObject in visualGameObjectsList)
        {
            Destroy(visualGameObject);
        }
        visualGameObjectsList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        float eulerZ = 0f;
        switch (e.line.oriyentation)
        {
            default:
            case GameManager.Oriyentation.Horizontal:
                eulerZ = 0f;
                break;
            case GameManager.Oriyentation.Vertical:
                eulerZ = 90f;
                break;
            case GameManager.Oriyentation.DiagonalA:
                eulerZ = 45f;
                break;
            case GameManager.Oriyentation.DiagonalB:
                eulerZ = -45f;
                break;

        }
        Transform lineCompleteTransform = Instantiate(lineCompletePrefab, GetGridWorldPosition(e.line.centerGridPosition.x, e.line.centerGridPosition.y), Quaternion.Euler(0, 0, eulerZ));
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjectsList.Add(lineCompleteTransform.gameObject);
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x, e.y, e.playerType);

    }
    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {

        Debug.Log("Spawned object");
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }
        Transform spawnedCrossTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjectsList.Add(spawnedCrossTransform.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
