using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Unit
{
    public bool isMoved = false;
    List<Coord> poses = new List<Coord>() { new Coord(1, 0), new Coord(1, 1), new Coord(1, -1), new Coord(0, 1), new Coord(0, -1), new Coord(-1, 1), new Coord(-1, 0), new Coord(-1, -1) };
    public override List<Node> GetMovableNode()
    {
        List<Node> movableNodes = new List<Node>();
        for (int i = 0; i < poses.Count; i++)
        {
            Coord pos = poses[i] + currentPos;
            if (!pos.IsOverBoard())
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }

        //Castling
        if (!isMoved)
        {
            //O-O
            if (unitManager.map[7,currentPos.y].currentUnit != null)
            {
                if (unitManager.map[7, currentPos.y].currentUnit.unitType == (UnitType)Enum.Parse(typeof(UnitType), unitColor.ToString() + "Rook") && !((Rook)unitManager.map[7, currentPos.y].currentUnit).isMoved)
                {
                    if (unitManager.map[5, currentPos.y].currentUnit == null && unitManager.map[6, currentPos.y].currentUnit == null)
                    {
                        movableNodes.Add(unitManager.map[6, currentPos.y]);
                    }
                }
            }

            //O-O-O
            if (unitManager.map[0, currentPos.y].currentUnit != null)
            {
                if (unitManager.map[0, currentPos.y].currentUnit.unitType == (UnitType)Enum.Parse(typeof(UnitType), unitColor.ToString() + "Rook") && !((Rook)unitManager.map[0, currentPos.y].currentUnit).isMoved)
                {
                    if (unitManager.map[1, currentPos.y].currentUnit == null && unitManager.map[2, currentPos.y].currentUnit == null && unitManager.map[3, currentPos.y].currentUnit == null)
                    {
                        movableNodes.Add(unitManager.map[2, currentPos.y]);
                    }
                }
            }
        }
        
        movableNodes = movableNodes.Where(_ => _.currentUnit == null || _.currentUnit.unitColor != unitColor).ToList();
        movableNodes = movableNodes.Where(_ => !Check_Illegalmove(_.pos)).ToList();

        return movableNodes;
    }
    public override bool Check_Illegalmove(Coord coord)
    {
        //Castling
        if(Coord.Distance(coord, currentPos) > 1.9)
        {
            bool isIllegalmove = false;

            Rook rook = null;
            Node rookNode;
            Node originRookNode;

            if (coord == new Coord(6, currentPos.y))
            {
                rook = (Rook)unitManager.map[7, currentPos.y].currentUnit;
                originRookNode = unitManager.map[7, currentPos.y];
                rookNode = unitManager.map[5, currentPos.y];
            }
            else
            {
                rook = (Rook)unitManager.map[0, currentPos.y].currentUnit;
                rookNode = unitManager.map[3, currentPos.y];
                originRookNode = unitManager.map[0, currentPos.y];
            }


            unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
            unitManager.map[coord.x, coord.y].currentUnit = this;
            Coord originCoord = currentPos;
            currentPos = coord;
            originRookNode.currentUnit = null;
            rookNode.currentUnit = rook;

            isIllegalmove = GameManager.Instance.Check_Check(unitColor);

            currentPos = originCoord;
            unitManager.map[currentPos.x, currentPos.y].currentUnit = this;
            unitManager.map[coord.x, coord.y].currentUnit = null;
            originRookNode.currentUnit = rook;
            rookNode.currentUnit = null;

            return isIllegalmove;
        }
        else
        {
            return base.Check_Illegalmove(coord);
        }
    }
    public override void MoveUnit(Coord pos, bool recordMove)
    {
        //castling
        if (!isMoved && Coord.Distance(pos, currentPos) > 1.9f)
        {
            //O-O
            if (pos == new Coord(6, currentPos.y))
            {
                recordManager.records.Add(new Record(unitColor, "O-O"));
                unitManager.map[7, currentPos.y].currentUnit.MoveUnit(new Coord(5, currentPos.y), false);
            }
            //O-O
            else
            {
                recordManager.records.Add(new Record(unitColor, "O-O-O"));
                unitManager.map[0, currentPos.y].currentUnit.MoveUnit(new Coord(3, currentPos.y), false);
            }

            unitManager.map[currentPos.x, currentPos.y].currentUnit = null;

            currentPos = pos;
            transform.position = new Vector3(unitManager.map[pos.x, pos.y].transform.position.x, 14, unitManager.map[pos.x, pos.y].transform.position.z);

            unitManager.map[pos.x, pos.y].currentUnit = this;
        }
        else
        {
            base.MoveUnit(pos, true);
        }
        isMoved = true;
    }
}