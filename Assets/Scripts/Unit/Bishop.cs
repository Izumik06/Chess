using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bishop : Unit
{
    public override List<Node> GetMovableNode()
    {
        List<Node> movableNodes = new List<Node>();

        // �� �˻�
        for (int i = 1, j = 1; i < 8; i++, j++)
        {
            Coord pos = new Coord(i + currentPos.x, j + currentPos.y);
            if (pos.IsOverBoard()) { break; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    movableNodes.Add(unitManager.map[pos.x, pos.y]);
                }
                break;
            }
            else
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }

        // �� �˻�
        for (int i = 1, j = -1; i < 8; i++, j--)
        {
            Coord pos = new Coord(i + currentPos.x, j + currentPos.y);
            if (pos.IsOverBoard()) { break; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                Debug.Log(unitManager.map[pos.x, pos.y].gameObject.name);
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    movableNodes.Add(unitManager.map[pos.x, pos.y]);
                }
                break;
            }
            else
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }

        // �� �˻�
        for (int i = -1, j = -1; i > -8; i--, j--)
        {
            Coord pos = new Coord(i + currentPos.x, j + currentPos.y);
            if (pos.IsOverBoard()) { break; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    movableNodes.Add(unitManager.map[pos.x, pos.y]);
                }
                break;
            }
            else
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }

        // �� �˻�
        for (int i = -1, j = 1; i < 8; i--, j++)
        {
            Coord pos = new Coord(i + currentPos.x, j + currentPos.y);
            if (pos.IsOverBoard()) { break; }
            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                if (unitManager.map[pos.x, pos.y].currentUnit.unitColor != unitColor)
                {
                    movableNodes.Add(unitManager.map[pos.x, pos.y]);
                }
                break;
            }
            else
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }

        //�ݼ� ����
        movableNodes = movableNodes.Where(_ => !Check_Illegalmove(_.pos)).ToList();
        return movableNodes;
    }
    void asdf(){
        
    }
}
