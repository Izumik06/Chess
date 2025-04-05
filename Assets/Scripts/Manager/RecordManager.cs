using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    UnitManager unitManager;

    public List<Record> records = new List<Record>();
    public string fen;
    public int herfMove = 0;
    // Start is called before the first frame update
    void Start()
    {
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        fen = GetFEN();
    }
    /// <summary>
    /// FEN기보를 가져옴
    /// </summary>
    /// <returns></returns>
    public string GetFEN()
    {
        string fen = "";

        //현재 보드 위의 기물 위치
        for(int i = 7; i >= 0; i--)
        {
            int blankCount = 0;
            for (int j = 0; j < 8; j++)
            {
                Node node = GameManager.Instance.map[j, i];
                if (node.currentUnit != null)
                {
                    blankCount = 0;
                    string recordPiece = "";
                    if(node.currentUnit.unitType == UnitType.WhiteKnight || node.currentUnit.unitType == UnitType.BlackKnight)
                    {
                        recordPiece += "N";
                    }
                    else
                    {
                        recordPiece += node.currentUnit.unitType.ToString()[5];
                    }
                    if (node.currentUnit.unitColor == UnitColor.Black)
                    {
                        recordPiece = recordPiece.ToLower();
                    }
                    fen += recordPiece;
                }
                else if (j != 7 && GameManager.Instance.map[j + 1, i].currentUnit == null)
                {
                    blankCount++;
                }
                else
                {
                    fen += (blankCount + 1).ToString();
                }
            }
            if (i != 0) 
            {
                fen += '/';
            }
        }

        //턴 표기
        fen += " " + GameManager.Instance.turnPlayer.ToString().ToLower()[0] + " ";

        //캐슬링 가능 여부
        bool canCastling = false;
        King king = (King)unitManager.units[(int)UnitColor.White].Find(_ => _.unitType == UnitType.WhiteKing);
        if (!king.isMoved)
        {
            List<Unit> rooks = unitManager.units[(int)UnitColor.White].Where(_ => _ is Rook && !((Rook)_).isMoved).ToList();
            if(rooks.Count != 0)
            {
                if (rooks.Exists(_ => _.currentPos == new Coord(7, 0)))
                {
                    fen += "K";
                    canCastling = true;
                }
                if (rooks.Exists(_ => _.currentPos == new Coord(0, 0)))
                {
                    fen += "Q";
                    canCastling = true;
                }
            }
        }
        king = (King)unitManager.units[(int)UnitColor.Black].Find(_ => _.unitType == UnitType.BlackKing);
        if (!king.isMoved)
        {
            List<Unit> rooks = unitManager.units[(int)UnitColor.Black].Where(_ => _ is Rook && !((Rook)_).isMoved).ToList();
            if (rooks.Count != 0)
            {
                if (rooks.Exists(_ => _.currentPos == new Coord(7, 7)))
                {
                    fen += "k";
                    canCastling = true;
                }
                if (rooks.Exists(_ => _.currentPos == new Coord(0, 7)))
                {
                    fen += "q";
                    canCastling = true;
                }
            }
        }
        if (!canCastling)
        {
            fen += "-";
        }
        fen += " ";

        //앙파상 체크
        Unit pawn = unitManager.units[GameManager.Instance.turnPlayer == UnitColor.Black ? 1 : 0].Find(_ => _ is Pawn && ((Pawn)_).canEnpassant);
        if(pawn != null)
        {
            Coord position = new Coord(pawn.currentPos.x, (pawn.currentPos.y + (pawn.unitColor == UnitColor.Black ? 1 : -1)));
            fen += position.ToString();
        }
        else
        {
            fen += "-";
        }
        fen += " ";

        //둔 수
        fen += herfMove;
        fen += " ";
        fen += Mathf.CeilToInt(records.Count / 2 + 1);
        return fen;
    }
}

[Serializable]
public class Record
{
    public UnitColor color;
    public string recordText;

    public Record(UnitType unitType, Coord beforeCoord, Coord afterCoord, bool isKillUnit)
    {
        color = (UnitColor)Enum.Parse(typeof(UnitColor), unitType.ToString().Substring(0, 5));


        string unittype = unitType.ToString().Substring(5);

        switch (unittype) 
        {
            case "Pawn":
                if (isKillUnit)
                {
                    recordText += Convert.ToChar(beforeCoord.x + (int)'a') + "x";
                }
                break;
            case "Knight":
                recordText += "N";

                List<Coord> movableCoords = new List<Coord>() { new Coord(1, 2), new Coord(1, -2), new Coord(2, 1), new Coord(2, -1), new Coord(-1, 2), new Coord(-1, -2), new Coord(-2, 1), new Coord(-2, -1)};
                List<Coord> overlapCoords = new List<Coord>();
                for(int i = 0; i < movableCoords.Count; i++)
                {
                    Coord checkCoord = movableCoords[i] + afterCoord;
                    if (!checkCoord.IsOverBoard())
                    {
                        if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null)
                        {
                            if(GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType == unitType)
                            {
                                overlapCoords.Add(checkCoord);
                            }
                        }
                    }
                }


                if (overlapCoords.Count != 0)
                {
                    List<Coord> checkList = overlapCoords.Where(_ => _.y == beforeCoord.y).ToList();

                    bool isAddtext = false;
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                    checkList = overlapCoords.Where(_ => _.x == beforeCoord.x).ToList();
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += beforeCoord.y + 1;
                    }

                    if (!isAddtext)
                    {
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                }
                break;
            case "Rook":
                recordText += "R";

                for(int j = -1; j <= 1; j += 2)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        Coord checkCoord = new Coord(afterCoord.x + i * j, afterCoord.y);

                        if (!checkCoord.IsOverBoard())
                        {
                            if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null)
                            {
                                if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType == unitType)
                                {
                                    Debug.Log($"{checkCoord.x}{checkCoord.y}");
                                    recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                                    goto ExitRoop1;
                                }
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
ExitRoop1:
                for (int j = -1; j <= 1; j += 2)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        Coord checkCoord = new Coord(afterCoord.x, afterCoord.y + i * j);

                        if (!checkCoord.IsOverBoard())
                        {
                            if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null)
                            {
                                if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType == unitType)
                                {
                                    recordText += beforeCoord.y + 1;
                                    goto ExitRoop2;
                                }
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
ExitRoop2:
                break;
            case "Bishop":
                recordText += "B";
                List<Coord> overlapCoord = new List<Coord>();
                for (int i = -1; i < 2; i += 2)
                {
                    for (int j = 1; j < 8; j++)
                    {
                        Coord checkCoord = new Coord(afterCoord.x + j, afterCoord.y + j * i);

                        if (checkCoord.IsOverBoard()) { break; }
                        if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null)
                        {
                            if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null && GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType == unitType)
                            {
                                overlapCoord.Add(checkCoord);
                            }
                        }
                    }
                }
                for (int i = -1; i < 2; i += 2)
                {
                    for (int j = -1; j > -8; j--)
                    {
                        Coord checkCoord = new Coord(afterCoord.x + j, afterCoord.y + j * i);

                        if (checkCoord.IsOverBoard()) { break; }
                        if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null)
                        {
                            if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null && GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType == unitType)
                            {
                                overlapCoord.Add(checkCoord);
                            }
                        }
                    }
                }
                if (overlapCoord.Count != 0)
                {
                    List<Coord> checkList = overlapCoord.Where(_ => _.y == beforeCoord.y).ToList();

                    bool isAddtext = false;
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                    checkList = overlapCoord.Where(_ => _.x == beforeCoord.x).ToList();
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += beforeCoord.y + 1;
                    }

                    if (!isAddtext)
                    {
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                }
                break;
            case "Queen":
                recordText += "Q";
                break;
            case "King":
                recordText += "K";
                break;
        }
        if (unittype != "Pawn")
        {
            if (isKillUnit)
            {
                recordText += "x";
            }
        }
        recordText += $"{(char)(afterCoord.x + (int)'a')}{afterCoord.y + 1}";

        if (GameManager.Instance.Check_Check(1 - color))
        {
            if(GameManager.Instance.Check_Mate(1- color))
            {
                recordText += "#";
            }
            else
            {
                recordText += "+";
            }
        }

    }
    public Record(UnitColor color, string recordText)
    {
        this.color = color;
        this.recordText += recordText;
        if (GameManager.Instance.Check_Mate(1 - color))
        {
            recordText += "#";
        }
        else
        {
            recordText += "+";
        }
    }   
}