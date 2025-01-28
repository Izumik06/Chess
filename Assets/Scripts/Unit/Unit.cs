using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitType unitType;
    public UnitColor unitColor;
    public UnitManager unitManager;
    public int moveDir;
    public Coord currentPos;

    private void Start()
    {
        if((int)unitType < 6)
        {
            moveDir = 1;
            unitColor = UnitColor.White;
        }
        else
        {
            moveDir = -1;
            unitColor = UnitColor.Black;
        }
    }
    public void ShowMovableNode()
    {
        List<Node> movableNode = GetMovableNode();
        for (int i = 0; i < movableNode.Count; i++)
        {
            movableNode[i].gameObject.SetActive(true);
        }
    }
    public virtual void MoveUnit(Coord pos)
    {
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;

        currentPos = pos;
        transform.position = new Vector3(unitManager.map[pos.x, pos.y].transform.position.x, 14, unitManager.map[pos.x, pos.y].transform.position.z) ;

        if(unitManager.map[pos.x, pos.y].currentUnit != null)
        {
            unitManager.map[pos.x, pos.y].currentUnit.DestroyObject();
        }
        unitManager.map[pos.x, pos.y].currentUnit = this;

        GameManager.Instance.TurnChange();
    }
    public virtual bool Check_Illegalmove(Coord coord)
    {
        bool isIllegalmove = false;

        Unit unit = unitManager.map[coord.x, coord.y].currentUnit;
        unitManager.map[coord.x, coord.y].currentUnit = this;
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
        Coord originCoord = currentPos;
        currentPos = coord;
        unitManager.map[coord.x, coord.y].currentUnit = this;

        isIllegalmove = GameManager.Instance.Check_Check();

        currentPos = originCoord;
        unitManager.map[currentPos.x, currentPos.y].currentUnit = this;
        unitManager.map[coord.x, coord.y].currentUnit = unit;

        return isIllegalmove;
    }
    public void DestroyObject()
    {
        unitManager.units[(int)unitColor].Remove(this);
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
        for(int i = 0; i < 16; i++)
        {
            if (unitManager.unitCemetery[(int)unitColor][i].currentUnit == null)
            {
                transform.position = new Vector3(unitManager.unitCemetery[(int)unitColor][i].transform.position.x, 14, unitManager.unitCemetery[(int)unitColor][i].transform.position.z);
                unitManager.unitCemetery[(int)unitColor][i].currentUnit = this;
                break;
            }
        }
    }
    public virtual List<Node> GetMovableNode()
    {
        return null;
    }
}
[Serializable]
public class Coord
{
    public int x;
    public int y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public bool IsOverBoard()
    {
        if(x > 7 || x < 0 || y > 7 || y < 0)
        {
            return true;
        }
        return false;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public static Coord operator +(Coord a, Coord b)
    {
        return new Coord(a.x + b.x, a.y + b.y);
    }
    public static bool operator ==(Coord a, Coord b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Coord a, Coord b)
    {
        return a.x != b.x || a.y != b.y;
    }
    public static float Distance(Coord a, Coord b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        return (float)Math.Sqrt(num * num + num2 * num2);
    }
}
public enum UnitColor
{
    Black, White
}
