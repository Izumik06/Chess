using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knight : Unit
{
    List<Coord> poses = new List<Coord>() { new Coord(1,2), new Coord(1, -2), new Coord(-1, 2), new Coord(-1, -2), new Coord(2, 1), new Coord(2, -1), new Coord(-2, 1), new Coord(-2, -1) };
    public override List<Node> GetMovableNode()
    {
        List<Node> movableNodes = new List<Node>();
        for(int i = 0; i < poses.Count; i++)
        {
            Coord pos = poses[i] + currentPos;
            if(!pos.IsOverBoard())
            {
                movableNodes.Add(unitManager.map[pos.x, pos.y]);
            }
        }
        movableNodes = movableNodes.Where(_ => _.currentUnit == null || _.currentUnit.unitColor != unitColor).ToList();
        movableNodes = movableNodes.Where(_ => !Check_Illegalmove(_.pos)).ToList();
        return movableNodes;
        
    }
}