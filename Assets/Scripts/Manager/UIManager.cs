using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject recordBarPrefab;
    [SerializeField] List<GameObject> recordBars = new List<GameObject>();
    [SerializeField] Transform recordBoard;

    RecordManager recordManager;
    [SerializeField] List<Color> recordColor = new List<Color>();

    [SerializeField] TextMeshProUGUI player1Timer;
    [SerializeField] TextMeshProUGUI player2Timer;
    // Start is called before the first frame update
    void Start()
    {
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateTimerUI()
    {
        player1Timer.text = GameManager.Instance.player1Timer / 60 + ":" + GameManager.Instance.player1Timer % 60;
        player2Timer.text = GameManager.Instance.player2Timer / 60 + ":" + GameManager.Instance.player2Timer % 60;
    }
    public void SetRecord()
    {
        Record record = recordManager.records[recordManager.records.Count - 1];

        TextMeshProUGUI recordText = null;
        if(record.color == UnitColor.White)
        {
            GameObject recordbar = Instantiate(recordBarPrefab);
            recordbar.transform.parent = recordBoard;
            recordbar.GetComponent<Image>().color = recordColor[recordBars.Count % 2];

            recordbar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.CeilToInt(recordManager.records.Count / 2) + ".";
            recordText = recordbar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            recordBars.Add(recordbar);
        }
        else
        {
            recordText = recordBars[recordBars.Count - 1].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        recordText.text = record.recordText;
    }
}
