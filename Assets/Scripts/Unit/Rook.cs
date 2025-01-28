using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Rook : Unit
{
    public bool isMoved = false;
    public override List<Node> GetMovableNode()
    {
        List<Node> movableNodes = new List<Node>();
        for (int i = 1; i < 8; i++)
        {
            Coord pos = new Coord(i + currentPos.x, currentPos.y);
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
        for (int i = -1; i > -8; i--)
        {
            Coord pos = new Coord(i + currentPos.x, currentPos.y);
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
        for (int i = 1; i < 8; i++)
        {
            Coord pos = new Coord(currentPos.x, currentPos.y + i);
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
        for (int i = -1; i > -8; i--)
        {
            Coord pos = new Coord(currentPos.x, currentPos.y + i);
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
        movableNodes = movableNodes.Where(_ => !Check_Illegalmove(_.pos)).ToList();
        return movableNodes;
    }
    public override void MoveUnit(Coord pos)
    {
        base.MoveUnit(pos);
        isMoved = true;
    }
}
