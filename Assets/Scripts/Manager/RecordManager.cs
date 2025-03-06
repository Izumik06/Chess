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
    UnitColor color;
    public string recordText;

    public Record(UnitType unitType, Coord beforeCoord, Coord afterCoord, bool isKillUnit)
    {
        color = (UnitColor)Enum.Parse(typeof(UnitColor), unitType.ToString().Substring(0, 5));

        recordText = Mathf.Floor((GameObject.Find("RecordManager").GetComponent<RecordManager>().records.Count + 2) / 2) + (unitType.ToString().Contains("Black") ? "..." : ".");

        string unittype = unitType.ToString().Substring(5);

        switch (unittype) 
        {
            case "Pawn":
                if (isKillUnit)
                {
                    recordText += Convert.ToChar(beforeCoord.x + (int)'a') + "x";
                }
                recordText += $"{Convert.ToChar(afterCoord.x + (int)'a')}{afterCoord.y + 1}";
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
                        if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit != null && GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitColor == color)
                        {
                            if(GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Knight"))
                            {
                                overlapCoords.Add(checkCoord);
                            }
                        }
                    }
                }


                if(overlapCoords.Count != 0) {
                    List<Coord> checkList = overlapCoords.Where(_ => _.y == beforeCoord.y).ToList();

                    bool isAddtext = false;
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                    checkList = overlapCoords.Where(_ => _ == beforeCoord && _.x == beforeCoord.x).ToList();
                    if (checkList.Count > 0)
                    {
                        isAddtext = true;
                        recordText += beforeCoord.y + 1;
                    }

                    if(!isAddtext)
                    {
                        recordText += Convert.ToChar(beforeCoord.x + (int)'a');
                    }
                }


                recordText += $"{Convert.ToChar(afterCoord.x + (int)'a')}{afterCoord.y + 1}";
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
                                if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitColor == color && GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook"))
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
                                if (GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitColor == color && GameManager.Instance.map[checkCoord.x, checkCoord.y].currentUnit.unitType.ToString().Contains("Rook"))
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
                recordText += $"{Convert.ToChar(afterCoord.x + (int)'a')}{afterCoord.y + 1}";
                break;
            case "Bishop":
                recordText = "B";

                recordText += $"{(char)(afterCoord.x + (int)'a')}{afterCoord.y + 1}";
                break;
            case "Queen":
                break;
            case "King":
                break;
        }

        if (unitType.ToString().Contains("Pawn"))
        {

        }
        if (isKillUnit && unittype != "Pawn")
        {
            recordText.Insert(1, "x");
        }
        Debug.Log(recordText);
    }
    public Record(UnitColor color, string recordText)
    {
        this.color = color;
        this.recordText = recordText;
    }   
}