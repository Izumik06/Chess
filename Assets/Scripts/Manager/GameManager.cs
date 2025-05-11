using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;

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
            if (unitManager.units[(int)turnPlayer][i] is Pawn)
            {
                ((Pawn)unitManager.units[(int)turnPlayer][i]).canEnpassant = false;
            }
        }

        //표시된 체크 끄기
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                map[i, j].ResetNode();
            }
        }

        Node checkingNode;
        if (Check_Check(turnPlayer, out checkingNode))
        {
            //체크 표시
            Debug.Log(1);
            Debug.Log($"Node = {checkingNode.pos.x}{checkingNode.pos.y}");
            checkingNode.gameObject.SetActive(true);
            checkingNode.SetNodeStatus(NodeStatus.CheckingNode);
            checkingNode.ShowLine(unitManager.units[(int)turnPlayer].Find(_ =>_ is King).transform.position);

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
        List<Node> movableNodes = new List<Node>();
        foreach(Unit unit in unitManager.units[(int)unitColor])
        {
            movableNodes.AddRange(unit.GetMovableNode());
        }
        movableNodes = movableNodes.Distinct().ToList();
        bool canMove = false;
        foreach(Node node in movableNodes)
        {
            if(node.status != NodeStatus.CheckingNode)
            {
                node.gameObject.SetActive(false);
            }
            canMove = true;
        }
        return !canMove;
    }
    public bool Check_Check(UnitColor unitColor, out Node checkingNode)
    {
        King king = (King)unitManager.units[(int)unitColor].Find(_ => _ is King);
        Coord currentPos = king.currentPos;
        int moveDir = king.moveDir;
        

        //상하좌우 검사
        List<Coord> coords = new List<Coord> { new Coord(0, 1), new Coord(1, 0), new Coord(-1, 0), new Coord(0, -1) };
        for (int i = 0; i < 4; i++)
        {
            Coord pos = currentPos + coords[i];
            if (pos.IsOverBoard()) { continue; }
            if (map[pos.x, pos.y].currentUnit != null)
            {
                if (map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    if (map[pos.x, pos.y].currentUnit is Rook
                        || map[pos.x, pos.y].currentUnit is Queen
                        || map[pos.x, pos.y].currentUnit is King)
                    {
                        checkingNode = map[pos.x, pos.y];
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
            if (map[pos.x, pos.y].currentUnit != null)
            {
                if (map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    if (map[pos.x, pos.y].currentUnit is Bishop
                        || map[pos.x, pos.y].currentUnit is Queen
                        || map[pos.x, pos.y].currentUnit is King)
                    {
                        checkingNode = map[pos.x, pos.y];
                        return true;
                    }
                    if (pos == new Coord(currentPos.x + 1, currentPos.y + 1 * moveDir) && map[pos.x, pos.y].currentUnit is Pawn)
                    {
                        checkingNode = map[pos.x, pos.y];
                        return true;
                    }
                    else if (pos == new Coord(currentPos.x - 1, currentPos.y + 1 * moveDir) && map[pos.x, pos.y].currentUnit is Pawn)
                    {
                        checkingNode = map[pos.x, pos.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if(map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Rook
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if(map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Rook
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Rook
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Rook
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Bishop
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Bishop
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Bishop
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
            if (map[checkCoord.x, checkCoord.y].currentUnit != null)
            {
                if (map[checkCoord.x, checkCoord.y].currentUnit.unitColor != unitColor)
                {
                    if (map[checkCoord.x, checkCoord.y].currentUnit is Bishop
                        || map[checkCoord.x, checkCoord.y].currentUnit is Queen)
                    {
                        checkingNode = map[checkCoord.x, checkCoord.y];
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
                checkingNode = map[checkCoord.x, checkCoord.y];
                return true;
            }
        }
        checkingNode = null;
        return false;
    }
    public bool Check_Check(UnitColor color)
    {
        Node tempNode;
        return Check_Check(color, out tempNode);
    }
    public void ClearNode()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                map[i, j].RevertNode();
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
