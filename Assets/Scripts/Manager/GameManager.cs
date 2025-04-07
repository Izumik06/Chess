using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Drawing;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public UnitColor turnPlayer;
    public Node[,] map = new Node[8, 8];
    [SerializeField] Transform terrain;
    [SerializeField] UnitManager unitManager;

    public bool isOnPromotion = false;

    public GameObject nodePrefab;
    public float nodeSize;
    public float nodeHeight;
    public Vector3 node_startPos;

    UIManager uiManager;
    RecordManager recordManager;
    Stockfish stockfish;
    PlayerControl playerControl;

    public UnitColor player1Color;
    public float whiteTimer;
    public float blackTimer;
    public bool useBlackStockfish;
    public bool useWhiteStockfish;

    public bool isEndGame = false;
    public bool isStartGame = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        CreateMap();
    }
    private void Start()
    {
        playerControl = GameObject.Find("PlayerControl").GetComponent<PlayerControl>();
        stockfish = GameObject.Find("StockFish").GetComponent<Stockfish>();
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void Update()
    {
        if(isStartGame && !isEndGame)
        {
            if (whiteTimer > 0 && blackTimer > 0)
            {
                if (turnPlayer == UnitColor.White)
                {
                    whiteTimer -= Time.deltaTime;
                }
                else
                {
                    blackTimer -= Time.deltaTime;
                }
            }
            else
            {
                turnPlayer = 1 - turnPlayer;
                isEndGame = true;
                uiManager.SetResultText(false);
            }
        }
    }
    public void TurnChange()
    {
        if (turnPlayer == UnitColor.White)
        {
            turnPlayer = UnitColor.Black;
        }
        else
        {
            turnPlayer = UnitColor.White;
        }

        for (int i = 0; i < unitManager.units[(int)turnPlayer].Count; i++)
        {
            if (unitManager.units[(int)turnPlayer][i].unitType == (UnitType)Enum.Parse(typeof(UnitType), turnPlayer.ToString() + "Pawn"))
            {
                ((Pawn)unitManager.units[(int)turnPlayer][i]).canEnpassant = false;
            }
        }
        if (Check_Check(turnPlayer))
        {
            if (Check_Mate(turnPlayer))
            {
                recordManager.records[recordManager.records.Count - 1].recordText += "#";
            }
            else
            {
                recordManager.records[recordManager.records.Count - 1].recordText += "+";
            }
        }
        if (Check_Mate(turnPlayer))
        {
            Time.timeScale = 0;
            isEndGame = true;
            if (Check_Check(turnPlayer))
            {
                Debug.Log("체크메이트");
                uiManager.SetResultText(false);
            }
            else
            {
                uiManager.SetResultText(false);
                Debug.Log("스테일메이트");
            }
        }
        if(recordManager.herfMove >= 100)
        {
            isEndGame = true;
            uiManager.SetResultText(false);
            Debug.Log("무승부");
        }
        //다음 수를 스톡피쉬가 둘것인지
        useWhiteStockfish = uiManager.whiteStockfishToggle.isOn;
        useBlackStockfish = uiManager.blackStockfishToggle.isOn;
        if (!isEndGame)
        {
            if (turnPlayer == UnitColor.Black && useBlackStockfish)
            {
                stockfish.depth = (int)uiManager.blackStockfishSlider.value;
                playerControl.canPlayerControl = false;
                stockfish.MoveUnit();
            }
            else if (turnPlayer == UnitColor.White && useWhiteStockfish)
            {
                stockfish.depth = (int)uiManager.whiteStockfishSlider.value;
                playerControl.canPlayerControl = false;
                stockfish.MoveUnit();
            }
            else
            {
                playerControl.canPlayerControl = true;
            }
        }
        
        uiManager.SetRecord();
    }
    public void StartGame()
    {
        whiteTimer = int.Parse(uiManager.timeInput.text);
        blackTimer = int.Parse(uiManager.timeInput.text);

        useWhiteStockfish = uiManager.whiteStockfishToggle.isOn;
        useBlackStockfish = uiManager.blackStockfishToggle.isOn;
        if (turnPlayer == UnitColor.Black && useBlackStockfish)
        {
            stockfish.depth = (int)uiManager.blackStockfishSlider.value;
            playerControl.canPlayerControl = false;
            stockfish.MoveUnit();
        }
        else if (turnPlayer == UnitColor.White && useWhiteStockfish)
        {
            stockfish.depth = (int)uiManager.whiteStockfishSlider.value;
            playerControl.canPlayerControl = false;
            stockfish.MoveUnit();
        }
        else
        {
            playerControl.canPlayerControl = true;
        }
        isStartGame = true;
    }
    public bool Check_Mate(UnitColor unitColor)
    {
        for(int i = 0; i < unitManager.units[(int)unitColor].Count; i++)
        {
            unitManager.units[(int)unitColor][i].ShowMovableNode();
        }
        bool canMove = false;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (map[i, j].gameObject.activeSelf)
                {
                    canMove = true;
                    map[i, j].gameObject.SetActive(false);
                    continue;
                }
                map[i, j].gameObject.SetActive(false);
            }
        }

        if (canMove)
        {
            return false;
        }
        return true;
    }
    public bool Check_Check(UnitColor unitColor)
    {
        King king = unitManager.units[(int)unitColor].Where(_ => _.unitColor == unitColor && _.unitType.ToString().Contains("King")).ToList()[0].GetComponent<King>();
        Coord currentPos = king.currentPos;
        int moveDir = king.moveDir;
        

        //상하좌우 검사
        List<Coord> coords = new List<Coord> { new Coord(0, 1), new Coord(1, 0), new Coord(-1, 0), new Coord(0, -1) };
        for (int i = 0; i < 4; i++)
        {
            Coord pos = currentPos + coords[i];
            if (pos.IsOverBoard()) { continue; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Rook")
                        || unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Queen")
                        || unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("King"))
                    {
                        Debug.Log($"{pos.x}, {pos.y}");
                        return true;
                    }
                }
            }
        }

        //대각선 검사
        coords = new List<Coord> { new Coord(1, 1), new Coord(1, -1), new Coord(-1, 1), new Coord(-1, -1) };
        for (int i = 0; i < 4; i++)
        {
            Coord pos = currentPos + coords[i];
            if (pos.IsOverBoard()) { continue; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Bishop")
                        || unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Queen")
                        || unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("King"))
                    {
                        Debug.Log($"{pos.x}, {pos.y}");
                        return true;
                    }
                    if (pos == new Coord(currentPos.x + 1, currentPos.y + 1 * moveDir) && unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Pawn"))
                    {
                        Debug.Log($"{pos.x}, {pos.y}");
                        return true;
                    }
                    else if (pos == new Coord(currentPos.x - 1, currentPos.y + 1 * moveDir) && unitManager.map[pos.x, pos.y].currentUnit.unitType.ToString().Contains("Pawn"))
                    {
                        Debug.Log($"{pos.x}, {pos.y}");
                        return true;
                    }
                }
            }
        }

        //룩, 퀸 확인
        //상
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x, currentPos.y + i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if(unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{map[checkCoord.x, checkCoord.y].name}, {checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }
        //하
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x, currentPos.y - i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if(unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }
        //우
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x + i, currentPos.y);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }
        //좌
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x - i, currentPos.y);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    Debug.Log(2);
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }

        //비숍, 퀸 확인
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x + i, currentPos.y + i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Bishop")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x - i, currentPos.y - i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Bishop")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x - i, currentPos.y + i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Bishop")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;  
            }
        }
        for (int i = 1; i <= 8; i++)
        {
            Coord checkCoord = new Coord(currentPos.x + i, currentPos.y - i);
            if (checkCoord.IsOverBoard())
            {
                break;
            }
            if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Bishop")
                        || unitManager.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Queen"))
                    {
                        Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                        return true;
                    }
                }
                break;
            }
        }

        //나이트 검사
        coords = new List<Coord>() { new Coord(1, 2), new Coord(1, -2), new Coord(-1, 2), new Coord(-1, -2), new Coord(2, 1), new Coord(2, -1), new Coord(-2, 1), new Coord(-2, -1) };
        for(int i = 0; i < 8; i++)
        {
            Coord checkCoord = currentPos + coords[i];
            if (checkCoord.IsOverBoard()) { continue; }
            if (map[checkCoord.x, checkCoord.y].currentUnit != null && map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor && map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Knight"))
            {
                Debug.Log($"{checkCoord.x}, {checkCoord.y}");
                return true;
            }
        }

        return false;
    }
    public void ClearNode()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                map[i, j].gameObject.SetActive(false);
            }
        }
    }
    void CreateMap()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject node = Instantiate(nodePrefab);
                node.transform.parent = terrain;
                node.transform.localPosition = node_startPos + (new Vector3(-j, 0, i) * nodeSize);
                node.name = $"{Convert.ToChar(i + 'A')}{j + 1}";
                map[i, j] = node.GetComponent<Node>();
                map[i, j].pos = new Coord(i, j);
            }
        }
    }
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
