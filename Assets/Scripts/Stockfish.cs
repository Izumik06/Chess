using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Networking;

public class Stockfish : MonoBehaviour
{
    [Serializable]
    public class ApiAnswer
    {
        public bool success;
        public float evaluation;
        public string mate;
        public string bestmove;
        public string continuation;
    }
    public ApiAnswer answer;
    PlayerControl playerControl;
    RecordManager recordManager;
    public int depth = 12;

    Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        playerControl = GameObject.Find("PlayerControl").GetComponent<PlayerControl>();
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 0;
            StopCoroutine(coroutine);
        }
        if (GameManager.Instance.isEndGame)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
    public void MoveUnit()
    {
        coroutine = StartCoroutine(UnityWebRequestGet());
    }
    IEnumerator UnityWebRequestGet()
    {
        string url = $"https://stockfish.online/api/s/v2.php?fen={recordManager.GetFEN()}&depth={depth}";
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url); //요청

        yield return www.SendWebRequest();//요청 보내고 기다리기

        if(www.error == null)
        {
            answer = JsonUtility.FromJson<ApiAnswer>(www.downloadHandler.text);
            Coord unitPos = Coord.FromString(answer.bestmove.Substring(9, 2));
            Coord destination = Coord.FromString(answer.bestmove.Substring(11, 2));
            playerControl.selectedUnit = GameManager.Instance.map[unitPos.x, unitPos.y].currentUnit;
            playerControl.MoveUnit(destination);
            if (answer.bestmove.Length > 13 && answer.bestmove[13] != ' ')
            {
                int index = 0;
                switch (answer.bestmove[13])
                {
                    case 'q':
                        index = 3;
                        break;
                    case 'n':
                        index = 2;
                        break;
                    case 'b':
                        index = 1;
                        break;
                    case 'r':
                        index = 0;
                        break;
                }
                GameObject.Find(GameManager.Instance.turnPlayer.ToString() + "PromotionObj").transform.GetChild(index).GetComponent<PromotionObj>().Promotion();
            }
        }
        else
        {
            Debug.Log("실패");
        }
    }

}
