using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public Node[,] map;
    public List<List<Unit>> units = new List<List<Unit>>();
    public List<GameObject> unitPrefabs = new List<GameObject>();
    public List<Transform> unitCemeteryTerrain = new List<Transform>();
    public List<List<UnitCemetery>> unitCemetery = new List<List<UnitCemetery>>();

    [SerializeField] Transform blackPromotionObj;
    [SerializeField] Transform whitePromotionObj;
    // Start is called before the first frame update
    void Start()
    {
        map = GameManager.Instance.map;
        unitCemetery.Add(new List<UnitCemetery>());
        unitCemetery.Add(new List<UnitCemetery>());
        for (int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 16; j++)
            {
                unitCemetery[i].Add(unitCemeteryTerrain[i].GetChild(j).GetComponent<UnitCemetery>());
            }
        }
        CreateUnit();
    }

    void CreateUnit()
    {
        units.Add(new List<Unit>());
        units.Add(new List<Unit>());
        //Pawn
        for (int i = 0; i < 8; i++)
        {
            GameObject pawn = Instantiate(unitPrefabs[(int)UnitType.WhitePawn]);
            pawn.transform.position = new Vector3(map[i, 1].transform.position.x, 15f, map[i, 1].transform.position.z);
            pawn.GetComponent<Unit>().unitManager = this;
            pawn.GetComponent<Unit>().currentPos = new Coord(i, 1);
            pawn.GetComponent<Pawn>().blackPromotionObj = blackPromotionObj;
            pawn.GetComponent<Pawn>().whitePromotionObj = whitePromotionObj;
            map[i, 1].currentUnit = pawn.GetComponent<Unit>();
            units[(int)UnitColor.White].Add(pawn.GetComponent<Unit>());

        }
        for (int i = 0; i < 8; i++)
        {
            GameObject pawn = Instantiate(unitPrefabs[(int)UnitType.BlackPawn]);
            pawn.transform.position = new Vector3(map[i, 6].transform.position.x, 15f, map[i, 6].transform.position.z);
            pawn.GetComponent<Unit>().unitManager = this;
            pawn.GetComponent<Unit>().currentPos = new Coord(i, 6);
            pawn.GetComponent<Pawn>().blackPromotionObj = blackPromotionObj;
            pawn.GetComponent<Pawn>().whitePromotionObj = whitePromotionObj;
            map[i, 6].currentUnit = pawn.GetComponent<Unit>();
            units[(int)UnitColor.Black].Add(pawn.GetComponent<Unit>());
        }

        //Knight
        GameObject knight;
        knight = Instantiate(unitPrefabs[(int)UnitType.WhiteKnight]);
        knight.transform.position = new Vector3(map[1, 0].transform.position.x, 15f, map[1, 0].transform.position.z);
        knight.GetComponent<Unit>().unitManager = this;
        knight.GetComponent<Unit>().currentPos = new Coord(1, 0);
        map[1, 0].currentUnit = knight.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(knight.GetComponent<Unit>());

        knight = Instantiate(unitPrefabs[(int)UnitType.WhiteKnight]);
        knight.transform.position = new Vector3(map[6, 0].transform.position.x, 15f, map[6, 0].transform.position.z);
        knight.GetComponent<Unit>().unitManager = this;
        knight.GetComponent<Unit>().currentPos = new Coord(6, 0);
        map[6, 0].currentUnit = knight.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(knight.GetComponent<Unit>());

        knight = Instantiate(unitPrefabs[(int)UnitType.BlackKnight]);
        knight.transform.position = new Vector3(map[1, 7].transform.position.x, 15f, map[1, 7].transform.position.z);
        knight.GetComponent<Unit>().unitManager = this;
        knight.GetComponent<Unit>().currentPos = new Coord(1, 7);
        map[1, 7].currentUnit = knight.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(knight.GetComponent<Unit>());

        knight = Instantiate(unitPrefabs[(int)UnitType.BlackKnight]);
        knight.transform.position = new Vector3(map[6, 7].transform.position.x, 15f, map[6, 7].transform.position.z);
        knight.GetComponent<Unit>().unitManager = this;
        knight.GetComponent<Unit>().currentPos = new Coord(6, 7);
        map[6, 7].currentUnit = knight.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(knight.GetComponent<Unit>());

        //Bishop
        GameObject bishop;
        bishop = Instantiate(unitPrefabs[(int)UnitType.WhiteBishop]);
        bishop.transform.position = new Vector3(map[2, 0].transform.position.x, 15f, map[2, 0].transform.position.z);
        bishop.GetComponent<Unit>().unitManager = this;
        bishop.GetComponent<Unit>().currentPos = new Coord(2, 0);
        map[2, 0].currentUnit = bishop.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(bishop.GetComponent<Unit>());

        bishop = Instantiate(unitPrefabs[(int)UnitType.WhiteBishop]);
        bishop.transform.position = new Vector3(map[5, 0].transform.position.x, 15f, map[5, 0].transform.position.z);
        bishop.GetComponent<Unit>().unitManager = this;
        bishop.GetComponent<Unit>().currentPos = new Coord(5, 0);
        map[5, 0].currentUnit = bishop.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(bishop.GetComponent<Unit>());

        bishop = Instantiate(unitPrefabs[(int)UnitType.BlackBishop]);
        bishop.transform.position = new Vector3(map[2, 7].transform.position.x, 15f, map[2, 7].transform.position.z);
        bishop.GetComponent<Unit>().unitManager = this; 
        bishop.GetComponent<Unit>().currentPos = new Coord(2, 7);
        map[2, 7].currentUnit = bishop.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(bishop.GetComponent<Unit>());

        bishop = Instantiate(unitPrefabs[(int)UnitType.BlackBishop]);
        bishop.transform.position = new Vector3(map[5, 7].transform.position.x, 15f, map[5, 7].transform.position.z);
        bishop.GetComponent<Unit>().unitManager = this;
        bishop.GetComponent<Unit>().currentPos = new Coord(5, 7);
        map[5, 7].currentUnit = bishop.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(bishop.GetComponent<Unit>());

        //Rook
        GameObject rook;
        rook = Instantiate(unitPrefabs[(int)UnitType.WhiteRook]);
        rook.transform.position = new Vector3(map[0, 0].transform.position.x, 15f, map[0, 0].transform.position.z);
        rook.GetComponent<Unit>().unitManager = this;
        rook.GetComponent<Unit>().currentPos = new Coord(0, 0);
        map[0, 0].currentUnit = rook.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(rook.GetComponent<Unit>());

        rook = Instantiate(unitPrefabs[(int)UnitType.WhiteRook]);
        rook.transform.position = new Vector3(map[7, 0].transform.position.x, 15f, map[7, 0].transform.position.z);
        rook.GetComponent<Unit>().unitManager = this;
        rook.GetComponent<Unit>().currentPos = new Coord(7, 0);
        map[7, 0].currentUnit = rook.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(rook.GetComponent<Unit>());

        rook = Instantiate(unitPrefabs[(int)UnitType.BlackRook]);
        rook.transform.position = new Vector3(map[0, 7].transform.position.x, 15f, map[0, 7].transform.position.z);
        rook.GetComponent<Unit>().unitManager = this;
        rook.GetComponent<Unit>().currentPos = new Coord(0, 7);
        map[0, 7].currentUnit = rook.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(rook.GetComponent<Unit>());

        rook = Instantiate(unitPrefabs[(int)UnitType.BlackRook]);
        rook.transform.position = new Vector3(map[7, 7].transform.position.x, 15f, map[7, 7].transform.position.z);
        rook.GetComponent<Unit>().unitManager = this;
        rook.GetComponent<Unit>().currentPos = new Coord(7, 7);
        map[7, 7].currentUnit = rook.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(rook.GetComponent<Unit>());

        //Queen
        GameObject queen;
        queen = Instantiate(unitPrefabs[(int)UnitType.WhiteQueen]);
        queen.transform.position = new Vector3(map[3, 0].transform.position.x, 15f, map[3, 0].transform.position.z);
        queen.GetComponent<Unit>().unitManager = this;
        queen.GetComponent<Unit>().currentPos = new Coord(3, 0);
        map[3, 0].currentUnit = queen.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(queen.GetComponent<Unit>());

        queen = Instantiate(unitPrefabs[(int)UnitType.BlackQueen]);
        queen.transform.position = new Vector3(map[3, 7].transform.position.x, 15f, map[3, 7].transform.position.z);
        queen.GetComponent<Unit>().unitManager = this;
        queen.GetComponent<Unit>().currentPos = new Coord(3, 7);
        map[3, 7].currentUnit = queen.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(queen.GetComponent<Unit>());

        //King
        GameObject king;
        king = Instantiate(unitPrefabs[(int)UnitType.WhiteKing]);
        king.transform.position = new Vector3(map[4, 0].transform.position.x, 15f, map[4, 0].transform.position.z);
        king.GetComponent<Unit>().unitManager = this;
        king.GetComponent<Unit>().currentPos = new Coord(4, 0);
        map[4, 0].currentUnit = king.GetComponent<Unit>();
        units[(int)UnitColor.White].Add(king.GetComponent<Unit>());

        king = Instantiate(unitPrefabs[(int)UnitType.BlackKing]);
        king.transform.position = new Vector3(map[4, 7].transform.position.x, 15f, map[4, 7].transform.position.z);
        king.GetComponent<Unit>().unitManager = this;
        king.GetComponent<Unit>().currentPos = new Coord(4, 7);
        map[4, 7].currentUnit = king.GetComponent<Unit>();
        units[(int)UnitColor.Black].Add(king.GetComponent<Unit>());

        Debug.Log(units[0].Count + ", " + units[1].Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public enum UnitType
{
    WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing, BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing
}