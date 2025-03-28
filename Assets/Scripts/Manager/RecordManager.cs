using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public List<Record> records = new List<Record>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                break;
            case "King":
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