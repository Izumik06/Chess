using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public bool canPlayerControl = true;
    public Unit selectedUnit;
    public LayerMask unitLayer;
    public LayerMask nodeLayer;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isStartGame && !GameManager.Instance.isEndGame)
        {
            //선택된 기물을 마우스 위치로 이동
            if (selectedUnit != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Plane plane = new Plane(Vector3.up, new Vector3(0, 14, 0));

                float rayLenght;
                if (plane.Raycast(ray, out rayLenght))
                {
                    Vector3 mousePoint = ray.GetPoint(rayLenght);

                    selectedUnit.transform.position = new Vector3(mousePoint.x, mousePoint.y, mousePoint.z);
                }
            }
            //플레이어 기물 선택
            if (Input.GetKeyDown(KeyCode.Mouse0) && canPlayerControl)
            {
                if (selectedUnit == null)
                {
                    SelectUnit();
                }
            }
            //플레이어 기물 이동
            if (Input.GetKeyUp(KeyCode.Mouse0) && canPlayerControl)
            {
                if (selectedUnit != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, nodeLayer))
                    {
                        MoveUnit(hit.transform.GetComponent<Node>().pos);
                    }
                    else //노드 클릭 안했을때
                    {
                        selectedUnit.transform.position = new Vector3(GameManager.Instance.map[selectedUnit.currentPos.x, selectedUnit.currentPos.y].transform.position.x, 14, GameManager.Instance.map[selectedUnit.currentPos.x, selectedUnit.currentPos.y].transform.position.z);
                        selectedUnit.GetComponent<Collider>().isTrigger = false;
                        selectedUnit = null;
                        GameManager.Instance.ClearNode();
                    }
                }
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
    public void MoveUnit(Coord pos)
    {
        GameManager.Instance.ClearNode();

        //이동 후에도 update에 있는 마우스 위치 이동코드로 인해 잘못된 위치로 가는 상황 예방
        Unit unit = selectedUnit;
        selectedUnit = null;

        unit.GetComponent<Collider>().isTrigger = false;
        unit.MoveUnit(pos, true);

    }
}
