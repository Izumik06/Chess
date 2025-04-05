using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PromotionObj : MonoBehaviour
{
    [SerializeField] UnitManager unitManager;
    public UnitType unitType;
    public LayerMask layer;
    public UnitColor color;
    private void OnMouseDown()
    {
        if (color == UnitColor.White)
        {
            Pawn pawn = (Pawn)unitManager.units[(int)color].Where(_ => _.currentPos.y == 7 && _.unitType == UnitType.WhitePawn).ToList()[0];
            pawn.Promotion(unitType);
        }
        else
        {
            Pawn pawn = (Pawn)unitManager.units[(int)color].Where(_ => _.currentPos.y == 0 && _.unitType == UnitType.BlackPawn).ToList()[0];
            pawn.Promotion(unitType);
        }
        char promotionInitial;
        if (unitType.ToString().Contains("Knight"))
        {
            promotionInitial = 'N';
        }
        else
        {
            promotionInitial = unitType.ToString()[5];
        }
        transform.parent.gameObject.SetActive(false);
        GameObject.Find("RecordManager").GetComponent<RecordManager>().records[GameObject.Find("RecordManager").GetComponent<RecordManager>().records.Count - 1].recordText += "=" + promotionInitial;
        GameManager.Instance.TurnChange();
    }
}
