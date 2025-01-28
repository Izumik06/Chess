using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    public Unit selectedUnit;
    public LayerMask unitLayer;
    public LayerMask nodeLayer;

    // Update is called once per frame
    void Update()
    {
        if(selectedUnit != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, new Vector3(0,14,0));

            float rayLenght;
            if (plane.Raycast(ray, out rayLenght))
            {
                Vector3 mousePoint = ray.GetPoint(rayLenght);

                selectedUnit.transform.position = new Vector3(mousePoint.x, mousePoint.y, mousePoint.z);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {    
            if (selectedUnit == null)
            {
                SelectUnit();
            }
            else
            {
                MoveUnit();
            }
        }
    }
    void SelectUnit()
    {
        if (GameManager.Instance.isOnPromotion) { return; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if(unit.unitColor == GameManager.Instance.turnPlayer)
            {
                selectedUnit = unit;
                selectedUnit.GetComponent<Collider>().isTrigger = true;
                selectedUnit.ShowMovableNode();
            }
        }
    }

    void MoveUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayer))
        {
            GameManager.Instance.ClearNode();
            selectedUnit.GetComponent<Collider>().isTrigger = false;
            selectedUnit.MoveUnit(hit.transform.GetComponent<Node>().pos);
            selectedUnit = null;
        }
        else
        {
            selectedUnit.transform.position = new Vector3(GameManager.Instance.map[(int)selectedUnit.currentPos.x, (int)selectedUnit.currentPos.y].transform.position.x, 14, GameManager.Instance.map[(int)selectedUnit.currentPos.x, (int)selectedUnit.currentPos.y].transform.position.z);
            selectedUnit.GetComponent<Collider>().isTrigger = false;
            selectedUnit = null;
            GameManager.Instance.ClearNode();
        }
    }
}
