using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject recordBarPrefab;
    [SerializeField] List<GameObject> recordBars = new List<GameObject>();
    [SerializeField] Transform recordBoard;

    RecordManager recordManager;
    [SerializeField] List<Color> recordColor = new List<Color>();

    [SerializeField] TextMeshProUGUI whiteTimer;
    [SerializeField] TextMeshProUGUI blackTimer;
    public Toggle whiteStockfishToggle;
    public Toggle blackStockfishToggle;

    public Slider whiteStockfishSlider;
    public Slider blackStockfishSlider;

    [SerializeField] GameObject beforeStartUI;

    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject endUI;
    public TMP_InputField timeInput;
    // Start is called before the first frame update
    void Start()
    {
        recordManager = GameObject.Find("RecordManager").GetComponent<RecordManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimerUI();
    }
    void UpdateTimerUI()
    {
        whiteTimer.text = ((int)GameManager.Instance.whiteTimer / 60).ToString("00") + ":" + ((int)GameManager.Instance.whiteTimer % 60).ToString("00");
        blackTimer.text = ((int)GameManager.Instance.blackTimer / 60).ToString("00") + ":" + ((int)GameManager.Instance.blackTimer % 60).ToString("00");
    }
    public void SetResultText(bool isDraw)
    { 
        if (isDraw)
        {
            resultText.text = "Draw";
        }
        else
        {
            resultText.text = ((UnitColor)(1 - GameManager.Instance.turnPlayer)).ToString() + " Win!";
        }
        endUI.SetActive(true);
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

            recordbar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.CeilToInt(recordManager.records.Count / 2 + 1) + ".";
            recordText = recordbar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            recordBars.Add(recordbar);
        }
        else
        {
            recordText = recordBars[recordBars.Count - 1].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        recordText.text = record.recordText;
    }
    public void StartBtn()
    {
        if (timeInput.text == "") { return; }
        GameManager.Instance.StartGame();
        beforeStartUI.SetActive(false);
    }
    public void RestartBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
