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
        transform.parent.gameObject.SetActive(false);
    }
}
