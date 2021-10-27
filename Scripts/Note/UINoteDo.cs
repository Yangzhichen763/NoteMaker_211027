using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoteDo : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;


    [SerializeField] private UIBoxController boxController;
    [SerializeField] private UICreateController createController;
    private RectTransform boxTrans,createTrans;

    // Editor ����
    [SerializeField] private RectTransform showNoteBit;

    /// <summary> �� boxController �� position �� createController �� position �ľ����ٳ������� size.x </summary>
    private int noteScaleTimes = 0;
    public void Start()
    {
        boxTrans = boxController.GetComponent<RectTransform>();
        createTrans = createController.GetComponent<RectTransform>();

        noteScaleTimes = 0;
    }
    public void Update()
    {
        int times = GetTimes();

        if (noteScaleTimes > times)
        {
            noteScaleTimes = times;
        }

        while (noteScaleTimes < times)
        {
            if (noteScaleTimes < createController.GetPrefebs.Count)
                foreach (RectTransform rectTrans in createController.GetPrefebs[noteScaleTimes])
                {
                    if (rectTrans != null)
                    {
                        rectTrans.ScaleEase(Vector3.one, Vector3.one * 1.5f, 0.2f, 0.01f, ScaleEase,
                              () => rectTrans.localScale = Vector3.one);

                        AudioSource.PlayClipAtPoint(AudioPlayer.player.GetRandomClip(AudioPlayer.AudioType.Bubble),
                            Camera.main.transform.position);
                    }
                }

            ++noteScaleTimes;
        }
    }
    private int GetTimes()
    {
        Vector2 noteScale = createController.GetNoteScale();
        float delta_x = createController.Position.x - boxTrans.anchoredPosition.x + noteScale.x * 0.9f;
        int noteScaleTimes = (int)(delta_x / noteScale.x);

        return noteScaleTimes;
    }
    private float ScaleEase(float t)
        => curve.Evaluate(t);
}
