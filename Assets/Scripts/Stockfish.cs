using System;
using System.Collections;
using System.Collections.Generic;
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
    RecordManager recordManager;
    public int depth = 12;
    // Start is called before the first frame update
    void Start()
    {
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(UnityWebRequestGet());
        }
    }
    IEnumerator UnityWebRequestGet()
    {
        string url = $"https://stockfish.online/api/s/v2.php?fen={recordManager.GetFEN()}&depth={depth}";

        UnityWebRequest www = UnityWebRequest.Get(url); //요청

        yield return www.SendWebRequest();//보낸 뒤 응답 요청까지 기다림

        if(www.error == null)
        {
            answer = JsonUtility.FromJson<ApiAnswer>(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("ㅈ됨");
        }
    }

}
