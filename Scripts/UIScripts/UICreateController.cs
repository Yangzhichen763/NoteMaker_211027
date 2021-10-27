using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICreateController : MonoBehaviour
{
    public readonly int noteLine = 5;
    [SerializeField] private AnimationCurve noteUpCurve, noteDownCurve;


    [SerializeField] private bool isMiniMap = false;

    // ��ȡ��
    [SerializeField] private UIBoxController boxController;

    // �������
    [SerializeField] private RectTransform notePrefeb;
    [SerializeField] private int prefebCount = 25;
    private Queue<RectTransform> notePrefebPool;

    // ��¼��
    private List<RectTransform[]> prefebInput;
    private List<bool[]> boolInput;
    private bool lastIsCreate = false;

    public List<RectTransform[]> GetPrefebs => prefebInput;
    public List<bool[]> GetBools => boolInput;

    // �¼���
    public delegate void ChangeNote(int line, int column, int inputIndex);
    public event ChangeNote changeData;

    // ������
    private RectTransform thisTrans;
    public Vector2 Position => thisTrans.anchoredPosition
        - thisTrans.sizeDelta * thisTrans.pivot
        + 0.5f * new Vector2(ScreenScaler.developWidth, ScreenScaler.developHeight);

    public void Start()
    {
        thisTrans ??= GetComponent<RectTransform>();

        // �����
        if (notePrefeb.gameObject.activeInHierarchy)
            notePrefeb.gameObject.SetActive(false);
        notePrefebPool = new Queue<RectTransform>();

        int tmpCount = 0;
        while (++tmpCount <= prefebCount)
        {
            InstantiatePrefeb(out _);
        }

        // ��ʼ��
        prefebInput = new List<RectTransform[]>();
        boolInput = new List<bool[]>();
    }
    private void InstantiatePrefeb(out RectTransform outPrefeb)
    {
        outPrefeb = Instantiate(notePrefeb, boxController.Panel, false);
        outPrefeb.SetAsLastSibling();

        notePrefebPool.Enqueue(outPrefeb);
    }
    private RectTransform GetPrefeb()
    {
        RectTransform tmpTrans;
        int loopTimes = 0;
        do
        {
            tmpTrans = notePrefebPool.Dequeue();
            notePrefebPool.Enqueue(tmpTrans);
        } while (tmpTrans.gameObject.activeInHierarchy && ++loopTimes < notePrefebPool.Count);

        if (tmpTrans.gameObject.activeInHierarchy)
            InstantiatePrefeb(out tmpTrans);

        return tmpTrans;
    }
    public void Clear()
    {
        for (int i = 0; i < boolInput.Count; ++i)
        {
            for (int j = 0; j < noteLine; ++j)
            {
                if (boolInput[i][j])
                {
                    prefebInput[i][j].gameObject.SetActive(false);
                }
            }
        }
        boolInput = new List<bool[]>();
        prefebInput = new List<RectTransform[]>();

        lastIsCreate = false;
    }

    public void Update()
    {
        if (isMiniMap)
        {
            return;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetNotePrefebInfo(out int column, out int line);

                if (CheckIsInRange(Input.mousePosition))
                {
                    changeData?.Invoke(line, column, 0);
                    InputMouseDown(line, column);
                }
            }
            else if (Input.GetMouseButton(0) && lastIsCreate)
            {
                if (CheckIsInRange(Input.mousePosition))
                {
                    GetNotePrefebInfo(out int column, out int line);

                    changeData?.Invoke(line, column, 1);
                    InputMouse(line, column);
                }
            }
            else if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
            {
                if (CheckIsInRange(Input.mousePosition))
                {
                    GetNotePrefebInfo(out int column, out int line);

                    if (column < boolInput.Count
                        && boolInput[column][line])
                    {
                        changeData?.Invoke(line, column, 2);
                        InputRemoveMouse(line, column);
                    }
                }
            }
        }
    }

    public void InputMouseDown(int line, int column)
    {
        // �����б�
        InputIncrease(column);

        // �ж���û�з�������
        if (boolInput[column][line])
            RemoveNote(line, column);
        else
            CreateNote(line, column);
    }
    public void InputMouse(int line, int column)
    {
        // �����б�
        InputIncrease(column);

        if (!boolInput[column][line])
            CreateNote(line, column);
    }
    public void InputRemoveMouse(int line, int column)
    {
        RemoveNote(line, column);
    }


    /// <summary> �����б� </summary>
    /// <param name="column"> ����� </param>
    private void InputIncrease(int column)
    {
        while (boolInput.Count <= column)
        {
            boolInput.Add(new bool[noteLine]);
            prefebInput.Add(new RectTransform[noteLine]);
        }
    }
    /// <summary> �Ƴ�����ʵ�� </summary>
    /// <param name="line"> ������ </param>
    /// <param name="column"> ������ </param>
    private void RemoveNote(int line, int column)
    {
        RectTransform rectTrans = prefebInput[column][line];

        // ���ݶ���
        rectTrans.ScaleEase(Vector3.one, Vector3.zero, 0.25f, 0.01f, ScaleDownEase,
                              () =>
                              {
                                  rectTrans.localScale = Vector3.zero;

                                  rectTrans.gameObject.SetActive(false);
                              });

        AudioSource.PlayClipAtPoint(AudioPlayer.player.GetRandomClip(AudioPlayer.AudioType.Bubble),
            Camera.main.transform.position);


        prefebInput[column][line] = null;
        boolInput[column][line] = false;

        lastIsCreate = false;
    }
    /// <summary> ��������ʵ�� </summary>
    /// <param name="line"> ������ </param>
    /// <param name="column"> ������ </param>
    private void CreateNote(int line, int column)
    {
        RectTransform rectTrans = GetPrefeb();
        rectTrans.gameObject.SetActive(true);
        rectTrans.localPosition = new Vector2(column * notePrefeb.sizeDelta.x, line * notePrefeb.sizeDelta.y)
            + rectTrans.sizeDelta * rectTrans.pivot;


        // ���ݶ���
        rectTrans.ScaleEase(Vector3.zero, Vector3.one, 0.2f, 0.01f, ScaleUpEase,
                              () => rectTrans.localScale = Vector3.one);

        AudioSource.PlayClipAtPoint(AudioPlayer.player.GetRandomClip(AudioPlayer.AudioType.Bubble),
            Camera.main.transform.position);


        prefebInput[column][line] = rectTrans;
        boolInput[column][line] = true;


        lastIsCreate = true;
    }
    /// <summary> ��ȡ����ʵ������λ�ã���ת��Ϊ����λ�� </summary>
    /// <param name="column"></param>
    /// <param name="line"></param>
    private void GetNotePrefebInfo(out int column, out int line)
    {
        Vector2 size = GetNoteScale();
        Vector2 mousePos = ScreenScaler.ToUserPoint(Input.mousePosition);
        Vector2 backPos = boxController.Panel.anchoredPosition;
        Vector2 deltaVec = mousePos - backPos;

        column = (int)(deltaVec.x / size.x);
        line = (int)(deltaVec.y / size.y);
    }

    private bool CheckIsInRange(Vector2 inputPos)
    {
        inputPos = ScreenScaler.ToUserPoint(inputPos);
        Vector4 range = boxController.Range;
        return inputPos.x > range.x && inputPos.x < range.z
            && inputPos.y > range.y && inputPos.y < range.w;
    }

    /// <summary> ��ȡ�������ͺŴ�С </summary>
    /// <returns> ������ size </returns>
    public Vector2 GetNoteScale()
    {
        Vector2 size;
        size.x = notePrefeb.sizeDelta.x * boxController.Scale.x;
        size.y = notePrefeb.sizeDelta.y * boxController.Scale.y;

        return size;
    }
    private float ScaleUpEase(float t)
        => noteUpCurve.Evaluate(t);
    private float ScaleDownEase(float t)
         => noteDownCurve.Evaluate(t);
}
