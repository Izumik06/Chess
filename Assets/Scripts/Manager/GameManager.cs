using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

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

    public UnitColor player1Color;
    public int player1Timer;
    public int player2Timer;
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
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        StartCoroutine(UpdateTimer());
    }
    
    IEnumerator UpdateTimer()
    {
        yield return new WaitForSeconds(1f);
        if (turnPlayer == player1Color)
        {
            player1Timer -= 1;
        }
        else
        {
            player2Timer -= 1;
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
        uiManager.SetRecord();
        if (Check_Mate())
        {
            if (Check_Check())
            {
                Debug.Log("체크메이트");
            }
            else
            {
                Debug.Log("스테일메이트");
            }
        }
    }
    public bool Check_Mate()
    {
        for(int i = 0; i < unitManager.units[(int)turnPlayer].Count; i++)
        {
            unitManager.units[(int)turnPlayer][i].ShowMovableNode();
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
    public bool Check_Check()
    {
        King king = unitManager.units[(int)turnPlayer].Where(_ => _.unitColor == turnPlayer && _.unitType.ToString().Contains("King")).ToList()[0].GetComponent<King>();
        Coord currentPos = king.currentPos;
        UnitColor unitColor = king.unitColor;
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
