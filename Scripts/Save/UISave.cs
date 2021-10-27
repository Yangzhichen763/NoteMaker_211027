using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISave : MonoBehaviour
{
    [SerializeField] private string defaultPath;
    [SerializeField] private string fileName;

    [SerializeField] private UICreateController creator;
    [SerializeField] private UISetSpeed speedManager;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button saveButton;

    [Header("Message 部分")]
    [SerializeField] private UITipsPool tips;
    [SerializeField] private float messageStayTime = 2;

    private string inputStr;
    public void Start()
    {
        inputField.onEndEdit.AddListener((string input) => inputStr = input);

        saveButton ??= GetComponent<Button>();
        saveButton.onClick.AddListener(() => Save(inputStr));
    }
    public void Save(string path)
    {
        if (string.IsNullOrEmpty(path)) return;


        int deltaTick = 50 * 60 / speedManager.Speed;


        List<NoteTimeline> noteTimelineList = new List<NoteTimeline>();

        List<RectTransform[]> creatorTrans = creator.GetPrefebs;
        for (int i = 0; i < creatorTrans.Count; ++i)
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            for (int j = 0; j < creator.noteLine; ++j)
            {
                if (creator.GetBools[i][j])
                    strBuilder.Append(j);
            }

            string boardIndex = strBuilder.ToString();
            if (boardIndex != "")
            {
                NoteTimeline noteTimeLine = new NoteTimeline { tick = i * deltaTick, boardIndex = boardIndex };
                noteTimelineList.Add(noteTimeLine);
            }
        }

        JsonDo.SaveToJson(noteTimelineList, defaultPath, fileName, $"{inputStr}.json");


        StartCoroutine(PrefebDisplay($"{defaultPath}/{fileName}/{inputStr}.json"));
    }
    private IEnumerator PrefebDisplay(string path)
    {
        CanvasGroup canvas = tips.PrefebDisplay();
        canvas.transform.GetChild(0).GetComponent<Text>().text = $"<color=green>数据存储成功</color>!存储地址为: {path}";

        WaitForFixedUpdate second = new WaitForFixedUpdate();
        float time = 0;
        while(time < messageStayTime)
        {
            time += Time.fixedDeltaTime;

            yield return second;
        }

        tips.PrefebDisappear(canvas);
    }
}

[System.Serializable]
public class NoteTimeline
{
    public int tick;
    public string boardIndex;
}
