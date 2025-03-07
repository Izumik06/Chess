using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected RecordManager recordManager;
    public UnitManager unitManager;

    public UnitType unitType;
    public UnitColor unitColor;
    public Coord currentPos;

    //�������� ���Ǵ� ����
    //��, �鿡���� ���� �̵��ϴ� ����
    public int moveDir;

    private void Start()
    {
        if ((int)unitType < 6)
        {
            moveDir = 1;
            unitColor = UnitColor.White;
        }
        else
        {
            moveDir = -1;
            unitColor = UnitColor.Black;
        }
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
    }
    /// <summary>
    /// ���� �⹰�� �̵� ������ ĭ�� Ȱ��ȭ��Ŵ
    /// </summary>
    public void ShowMovableNode()
    {
        List<Node> movableNode = GetMovableNode();
        for (int i = 0; i < movableNode.Count; i++)
        {
            movableNode[i].gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// ���� �⹰�� �̵� ������ ĭ�� �������� �Լ�
    /// </summary>
    public virtual List<Node> GetMovableNode()
    {
        return null;
    }
    /// <summary>
    /// �ش� ��ǥ�� �⹰�� �̵�
    /// </summary>
    public virtual void MoveUnit(Coord pos)
    {
        Coord beforePos = currentPos;
        bool isKillUnit = unitManager.map[pos.x, pos.y].currentUnit != null;
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;

        currentPos = pos;
        transform.position = new Vector3(unitManager.map[pos.x, pos.y].transform.position.x, 14, unitManager.map[pos.x, pos.y].transform.position.z);

        if (unitManager.map[pos.x, pos.y].currentUnit != null)
        {
            unitManager.map[pos.x, pos.y].currentUnit.DestroyObject();
        }
        unitManager.map[pos.x, pos.y].currentUnit = this;

        //�⺸ ����
        recordManager.records.Add(new Record(unitType, beforePos, pos, isKillUnit));

        GameManager.Instance.TurnChange();
    }
    /// <summary>
    /// ���� ������ �ش� ��ǥ�� �̵��� �� �ִ��� Ȯ��
    /// </summary>
    public virtual bool Check_Illegalmove(Coord coord)
    {
        bool isIllegalmove = false;

        //�ش� ��ǥ�� �̵�
        Unit unit = unitManager.map[coord.x, coord.y].currentUnit;
        unitManager.map[coord.x, coord.y].currentUnit = this;
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
        Coord originCoord = currentPos;
        currentPos = coord;
        unitManager.map[coord.x, coord.y].currentUnit = this;

        //üũ���� Ȯ��
        isIllegalmove = GameManager.Instance.Check_Check();

        //�̵� ���� ��ǥ�� ����
        currentPos = originCoord;
        unitManager.map[currentPos.x, currentPos.y].currentUnit = this;
        unitManager.map[coord.x, coord.y].currentUnit = unit;

        return isIllegalmove;
    }
    /// <summary>
    /// �⹰�� ������ �� ����� �Լ�
    /// </summary>
    public void DestroyObject()
    {
        unitManager.units[(int)unitColor].Remove(this);
        unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
        for (int i = 0; i < 16; i++)
        {
            if (unitManager.unitCemetery[(int)unitColor][i].currentUnit == null)
            {
                transform.position = new Vector3(unitManager.unitCemetery[(int)unitColor][i].transform.position.x, 14, unitManager.unitCemetery[(int)unitColor][i].transform.position.z);
                unitManager.unitCemetery[(int)unitColor][i].currentUnit = this;
                break;
            }
        }
        gameObject.layer = 0;
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
        if (x > 7 || x < 0 || y > 7 || y < 0)
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
