using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }
    public event EventHandler CurrentPlayableplayerTypeChanged;
    public event EventHandler OnReMatch;
    public event EventHandler OnGameTied;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnObjectPlaced;

    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    public enum Oriyentation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB
    }

    public struct Line
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Oriyentation oriyentation;
    }
    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] playerTypeArray;
    private List<Line> lineList;
    private NetworkVariable<int> playerCrossScore = new NetworkVariable<int>();
    private NetworkVariable<int> playerCircleScore = new NetworkVariable<int>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Instance already exists");
            Destroy(gameObject);

        }
        playerTypeArray = new PlayerType[3, 3];
        lineList = new List<Line> {
            //Horizontal
new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0),},
centerGridPosition = new Vector2Int(1,0),
oriyentation = Oriyentation.Horizontal
},
new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1),},
centerGridPosition = new Vector2Int(1,1),
oriyentation = Oriyentation.Horizontal
},new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(2,2),},
centerGridPosition = new Vector2Int(1,2),
oriyentation = Oriyentation.Horizontal
},
            //Vertical
new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2),},
centerGridPosition = new Vector2Int(0,1)
,
oriyentation = Oriyentation.Vertical
},new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(1,2),},
centerGridPosition = new Vector2Int(1,1),
oriyentation = Oriyentation.Vertical
},new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(2,2),},
centerGridPosition = new Vector2Int(2,1),
oriyentation = Oriyentation.Vertical
},
            //Diagonal
new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,2),},
centerGridPosition = new Vector2Int(1,1),
oriyentation = Oriyentation.DiagonalA
},new Line {
gridVector2IntList = new List<Vector2Int> {new Vector2Int(2,0), new Vector2Int(1,1), new Vector2Int(0,2),},
centerGridPosition = new Vector2Int(1,1),
oriyentation = Oriyentation.DiagonalB
},
        };
    }

    public override void OnNetworkSpawn()
    {
        // Debug.Log("clinet id " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
        if (IsServer)
        {
            //currentPlayablePlayerType.Value = PlayerType.Cross;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            CurrentPlayableplayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };
        playerCrossScore.OnValueChanged += (int oldValue, int newValue) =>
        {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
        playerCircleScore.OnValueChanged += (int oldValue, int newValue) =>
        {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayablePlayerType.Value = PlayerType.Circle;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }


    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        if (playerType != currentPlayablePlayerType.Value)
        {
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None)
        {
            //already occupied
            return;
        }
        playerTypeArray[x, y] = playerType;
        TriggerOnObjectPlacedRpc();
        //Debug.Log(x + " " + y);
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType,
        });

        switch (currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }

        TestWinner();

    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnObjectPlacedRpc()
    {
        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
    }
    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine(
            playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
            );
    }


    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return
        aPlayerType != PlayerType.None &&
        aPlayerType == bPlayerType &&
        bPlayerType == cPlayerType;

    }
    private void TestWinner()
    {
        for (int i = 0; i < lineList.Count; i++)
        {
            Line line = lineList[i];
            // foreach (Line line in lineList)
            // {
            if (TestWinnerLine(line))
            {
                //win
                currentPlayablePlayerType.Value = PlayerType.None;
                PlayerType winPlayerType = playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y];
                switch (winPlayerType)
                {

                    case PlayerType.Cross:
                        playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        playerCircleScore.Value++;
                        break;
                }
                TriggerOnGameWinRpc(i, winPlayerType);
                return;
            }
        }
        bool hasTie = true;
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                if (playerTypeArray[x, y] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }
            }
        }
        if (hasTie)
        {
            TriggerOnGameTiedEventRpc();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTiedEventRpc()
    {
        OnGameTied?.Invoke(this, EventArgs.Empty);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType playerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = playerType,
        });

    }

    [Rpc(SendTo.Server)]
    public void ReMatchRpc()
    {

        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }
        currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnReMatchRpc();

    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnReMatchRpc()
    {
        OnReMatch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }

    public void GetScores(out int circleScore, out int crossScore)
    {
        crossScore = this.playerCrossScore.Value;
        circleScore = this.playerCircleScore.Value;

    }
}
