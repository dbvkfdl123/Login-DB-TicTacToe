using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlayerType { PlayerOne, PlayerTwo }

public enum GameState
{
    None,
    PlayerTurn,
    OpponentTurn,
    GameOver,
    OpponentDisconnect,
}

public enum Player { Player, Opponent}

public class TicTacToeManager : MonoBehaviour {

    public Sprite[] markSprites;

    public Sprite circleSprite; // O스프라이트
    public Sprite crossSprite;  // x스프라이트
    public GameObject[] cells; // cell 정보를 담을 배열
    public RectTransform GameOverPanel;

    private enum Mark { Circle, Cross };
    PlayerType playerType;
    int[] cellStates = new int[9];  //o,x 값 정보
    private TicTacToeNetwork networkManager;

    int rowNum = 3;

    //게임 상태
    GameState gameState;

    //게임 승패
    enum Winner { None, player,Opponent, Tie}

    private void Awake()
    {
        gameState = GameState.None;
    }

    private void Start()
    {
        networkManager = GetComponent<TicTacToeNetwork>();

        //cell 정보 배열 초기화
        for (int i = 0; i < cellStates.Length; i++)
        {
            cellStates[i] = -1;
        }
    }

    private void Update()
    {
        if (gameState == GameState.PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
                
                if (hitInfo)
                {
                    GameObject cell = hitInfo.transform.gameObject;
                    int cellIndex = int.Parse(cell.name);

                    //서버에 cell Index 전달
                    networkManager.DoPlayer(cellIndex);
                    //셀에 O 혹은 x 표시하기
                    DrawMark(cellIndex, Player.Player);
                }
            }
            
        }
    }

    public void DrawMark(int cellIdx, Player player)
    {
        Sprite markSprite;
        if (player == Player.Player)
        {
            markSprite = playerType == PlayerType.PlayerOne ? circleSprite : crossSprite;
            cellStates[cellIdx] = playerType == PlayerType.PlayerOne ? 0 : 1;
        }
        else
        {
            markSprite = playerType == PlayerType.PlayerOne ? crossSprite : circleSprite;
            cellStates[cellIdx] = playerType == PlayerType.PlayerOne ? 1 : 0;
        }
        // 해당 cell 에 sprite 할당하기
        //cells[cellIdx].GetComponent<SpriteRenderer>().sprite = markSprite;
        //할당된 cell 을 비활성화
        //cells[cellIdx].GetComponent<BoxCollider2D>().enabled = false;

        GameObject go = cells[cellIdx];
        go.GetComponent<SpriteRenderer>().sprite = markSprite;
        go.GetComponent<BoxCollider2D>().enabled = false;

        go.transform.DOScale(2, 0).OnComplete(() => 
        {
            go.transform.DOScale(1, 1).SetEase(Ease.OutBounce);
            go.GetComponent<SpriteRenderer>().DOFade(1, 1);

            Camera.main.DOShakePosition(0.5f, 1, 15);
        });

        // 턴 변경
        Winner result = CheckWinner();
        if (result == Winner.None)
        {
            if (gameState == GameState.PlayerTurn)
            {
                gameState = GameState.OpponentTurn;
            }
            else if(gameState == GameState.OpponentTurn)
            {
                gameState = GameState.PlayerTurn;
            }
        }
        else
        {
            gameState = GameState.GameOver;
            ShowGameOverPanel();
            //게임의 결과 화면에 표시
            if (result == Winner.player)
            {
                Debug.Log("Player Win!!");
            }
            else if (result == Winner.Opponent)
            {
                Debug.Log("Opponent Win!!");
            }
            else if (result == Winner.Tie)
            {
                Debug.Log("Tie");
            }
            else
            {
                Debug.Log("Draw");
            }
        }
    }

    Winner CheckWinner()
    {
        int playerTypeValue = (int)playerType;

        //가로체크
        for (int y = 0; y < rowNum; ++y)
        {
            int mark = cellStates[y * rowNum];
            int num = 0;
            for (int x = 0; x < rowNum; ++x)
            {
                int index = y * rowNum + x;
                if (mark == cellStates[index])
                {
                    ++num;
                }
            }
            if (mark != -1 && num == rowNum)
            {
                int markValue = (int)mark;
                return (playerTypeValue == markValue) ? Winner.player : Winner.Opponent;
            }
        }

        for (int x = 0; x < rowNum; ++x)
        {
            int mark = cellStates[x];
            int num = 0;
            for (int y = 0; y < rowNum; ++y)
            {
                int index = y * rowNum + x;
                if (mark == cellStates[index])
                {
                    ++num;
                }
            }
            if (mark != -1 && num == rowNum)
            {
                int markValue = (int)mark;
                return (playerTypeValue == markValue) ? Winner.player : Winner.Opponent;
            }
        }
        {
            int mark = cellStates[0];
            int num = 0;
            for (int xy = 0; xy < rowNum; ++xy)
            {
                int index = xy * rowNum + xy;
                if (mark == cellStates[index])
                {
                    ++num;
                }
            }
            if (mark!= -1 && num == rowNum)
            {
                int markValue = (int)mark;
                return (playerTypeValue == markValue) ? Winner.player : Winner.Opponent;
            }
        }
        {
            int mark = cellStates[rowNum -1];
            int num = 0;
            for (int xy = 0; xy < rowNum; ++xy)
            {
                int index = xy * rowNum + rowNum - xy -1;
                if (mark == cellStates[index])
                {
                    ++num;
                }
            }
            if (mark != -1 && num == rowNum)
            {
                int markValue = (int)mark;
                return (playerTypeValue == markValue) ? Winner.player : Winner.Opponent;
            }
        }
        {
            int num = 0;
            foreach (var cellState in cellStates)
            {
                if (cellState == -1)
                {
                    ++num;
                }
            }
            if (num == 0)
            {
                return Winner.Tie;
            }
        }
        return Winner.None;
    }

    public void StartGame(PlayerType player)
    {
        playerType = player;

        if (player == PlayerType.PlayerOne)
        {
            gameState = GameState.PlayerTurn;
        }
        else
        {
            gameState = GameState.OpponentTurn;
        }
    }
    
    public void ShowGameOverPanel()
    {
        GameOverPanel.DOLocalMoveY(0, 1).SetEase(Ease.InOutBack);
        GameOverPanel.GetComponent<CanvasGroup>().DOFade(1, 1);
    }
}
