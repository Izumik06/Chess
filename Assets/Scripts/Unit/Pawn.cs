using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Unit
{
    public Transform whitePromotionObj;
    public Transform blackPromotionObj;

    public bool canEnpassant = false;
    bool isMoved = false;                                                                                                               
    public override List<Node> GetMovableNode()
    {
        List<Node> movableNodes = new List<Node>();
       
        if(currentPos.x != 7)
        {
            if (unitManager.map[currentPos.x + 1, currentPos.y + 1 * moveDir].currentUnit != null && unitManager.map[currentPos.x + 1, currentPos.y + 1 * moveDir].currentUnit.unitColor != unitColor)
            {
                movableNodes.Add(unitManager.map[currentPos.x + 1, currentPos.y + 1 * moveDir]);
            }
        }
        if(currentPos.x != 0)
        {
            if (unitManager.map[currentPos.x - 1, currentPos.y + 1 * moveDir].currentUnit != null && unitManager.map[currentPos.x - 1, currentPos.y + 1 * moveDir].currentUnit.unitColor != unitColor)
            {
                movableNodes.Add(unitManager.map[currentPos.x - 1, currentPos.y + 1 * moveDir]);
            }
        }

        if(unitManager.map[currentPos.x, currentPos.y + 1 * moveDir].currentUnit == null)
        {
            movableNodes.Add(unitManager.map[currentPos.x, currentPos.y + 1 * moveDir]);
            if (!isMoved && unitManager.map[currentPos.x, currentPos.y + 2 * moveDir].currentUnit == null)
            {
                movableNodes.Add(unitManager.map[currentPos.x, currentPos.y + (2 * moveDir)]);
            }
        }

        //Enpassant
        if(currentPos.x != 7
            &&unitManager.map[currentPos.x + 1, currentPos.y].currentUnit != null 
            && unitManager.map[currentPos.x + 1, currentPos.y].currentUnit.unitType.ToString().Contains("Pawn") 
            && ((Pawn)unitManager.map[currentPos.x + 1, currentPos.y].currentUnit).canEnpassant)
        {
            movableNodes.Add(unitManager.map[currentPos.x + 1, currentPos.y + 1 * moveDir]);
        }
        if (currentPos.x != 0
            &&unitManager.map[currentPos.x - 1, currentPos.y].currentUnit != null
            && unitManager.map[currentPos.x - 1, currentPos.y].currentUnit.unitType.ToString().Contains("Pawn")
            && ((Pawn)unitManager.map[currentPos.x - 1, currentPos.y].currentUnit).canEnpassant)
        {
            movableNodes.Add(unitManager.map[currentPos.x - 1, currentPos.y + 1 * moveDir]);
        }

        movableNodes = movableNodes.Where(_ => _.currentUnit == null || _.currentUnit.unitColor != unitColor).ToList();
        movableNodes = movableNodes.Where(_ => !Check_Illegalmove(_.pos)).ToList();

        return movableNodes;
    }
    public override void MoveUnit(Coord pos)
    {
        if(Coord.Distance(pos, currentPos) > 1.9f)
        {
            canEnpassant = true;
        }
        //Enpassant
        if(pos.x != currentPos.x && unitManager.map[pos.x, pos.y].currentUnit == null)
        {
            unitManager.map[pos.x, currentPos.y].currentUnit.DestroyObject();
        }
        //Promotion
        if((unitColor == UnitColor.White && pos.y == 7) || (unitColor == UnitColor.Black && pos.y == 0))
        {
            //¿Ãµø
            unitManager.map[currentPos.x, currentPos.y].currentUnit = null;

            currentPos = pos;
            transform.position = new Vector3(unitManager.map[pos.x, pos.y].transform.position.x, 14, unitManager.map[pos.x, pos.y].transform.position.z);

            if (unitManager.map[pos.x, pos.y].currentUnit != null)
            {
                unitManager.map[pos.x, pos.y].currentUnit.DestroyObject();
            }
            unitManager.map[pos.x, pos.y].currentUnit = this;

            SetPromotionObj();
        }
        else
        {
            base.MoveUnit(pos);
        }

        isMoved = true;
    }
    public void Promotion(UnitType type)
    {
        GameObject unit = Instantiate(unitManager.unitPrefabs[(int)type]);
        unit.transform.position = new Vector3(transform.position.x, 14, transform.position.z);
        unit.GetComponent<Unit>().currentPos = currentPos;
        unit.GetComponent<Unit>().unitManager = unitManager;
        
        unitManager.units[(int)unitColor].Remove(this);
        unitManager.units[(int)unitColor].Add(unit.GetComponent<Unit>());

        unitManager.map[currentPos.x, currentPos.y].currentUnit = unit.GetComponent<Unit>();

        GameManager.Instance.TurnChange();
        GameManager.Instance.isOnPromotion = false; 
        Destroy(gameObject);
    }
    void SetPromotionObj()
    {
        GameManager.Instance.isOnPromotion = true;
        if(unitColor == UnitColor.Black)
        {
            blackPromotionObj.gameObject.SetActive(true);
            blackPromotionObj.position = new Vector3(transform.position.x, blackPromotionObj.position.y, blackPromotionObj.position.z);
        }
        else
        {
            whitePromotionObj.gameObject.SetActive(true);
            whitePromotionObj.position = new Vector3(transform.position.x, whitePromotionObj.position.y, whitePromotionObj.position.z);
        }
    }
    public override bool Check_Illegalmove(Coord coord)
    {
        //Enpassant
        if(Coord.Distance(coord, currentPos) > 1 && unitManager.map[coord.x, coord.y].currentUnit == null)
        {
            bool isIllegalmove = false;

            Unit unit = unitManager.map[coord.x, currentPos.y].currentUnit;
            unitManager.map[coord.x, coord.y].currentUnit = this;
            unitManager.map[coord.x, currentPos.y].currentUnit = null;
            unitManager.map[currentPos.x, currentPos.y].currentUnit = null;
            Coord originCoord = currentPos;
            currentPos = coord;

            isIllegalmove = GameManager.Instance.Check_Check();

            currentPos = originCoord;
            unitManager.map[currentPos.x, currentPos.y].currentUnit = this;
            unitManager.map[coord.x, coord.y].currentUnit = null;
            unitManager.map[coord.x, currentPos.y].currentUnit = unit;

            return isIllegalmove;
        }
        else
        {
            return base.Check_Illegalmove(coord);
        }
    }
}