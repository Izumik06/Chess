using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Unit selectedUnit;
    public LayerMask unitLayer;
    public LayerMask nodeLayer;

    // Update is called once per frame
    void Update()
    {
        //선택된 기물을 마우스 위치로 이동
        if (selectedUnit != null)
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
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if(selectedUnit != null)
            {
                MoveUnit();
            }
        }
    }
    /// <summary>
    /// 마우스 위치에 있는 기물을 선택
    /// </summary>
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

    /// <summary>
    /// 선택된 기물을 마우스 위치에 있는 노드로 이동
    /// </summary>
    void MoveUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayer))
        {
            GameManager.Instance.ClearNode();

            //이동 후에도 update에 있는 마우스 위치 이동코드로 인해 잘못된 위치로 가는 상황 예방
            Unit unit = selectedUnit;
            selectedUnit = null;

            unit.GetComponent<Collider>().isTrigger = false;
            unit.MoveUnit(hit.transform.GetComponent<Node>().pos);
        }
        //마우스의 위치에 활성화된 칸이 없을 시 원래 위치로 이동
        else
        {
            selectedUnit.transform.position = new Vector3(GameManager.Instance.map[(int)selectedUnit.currentPos.x, (int)selectedUnit.currentPos.y].transform.position.x, 14, GameManager.Instance.map[(int)selectedUnit.currentPos.x, (int)selectedUnit.currentPos.y].transform.position.z);
            selectedUnit.GetComponent<Collider>().isTrigger = false;
            selectedUnit = null;
            GameManager.Instance.ClearNode();
        }
    }
}
