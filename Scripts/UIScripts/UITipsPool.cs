using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITipsPool : MonoBehaviour
{
    [SerializeField] private CanvasGroup rectTransPrefeb;
    [SerializeField] private Transform parentTrans;
    [SerializeField] private int prefebCount = 4;
    private Queue<CanvasGroup> rectTransPool;

    [SerializeField] private float offset = 360;

    public void Start()
    {
        rectTransPool = new Queue<CanvasGroup>();
        if(rectTransPrefeb.gameObject.activeInHierarchy)
        rectTransPrefeb.gameObject.SetActive(false);

        int tmpPrefebCount = 0;
        while (++tmpPrefebCount <= prefebCount)
        {
            InstantiatePrefeb(out _);
        }
    }
    private void InstantiatePrefeb(out CanvasGroup canvas)
    {
        canvas = Instantiate(rectTransPrefeb, parentTrans);
        canvas.transform.SetAsLastSibling();

        rectTransPool.Enqueue(canvas);
    }
    public CanvasGroup GetPrefeb()
    {
        int tmpCount = 0;
        CanvasGroup rectCanvas;
        do
        {
            rectCanvas = rectTransPool.Dequeue();
            rectTransPool.Enqueue(rectCanvas);
        } while (rectCanvas.gameObject.activeInHierarchy && ++tmpCount < prefebCount);

        if (rectCanvas.gameObject.activeInHierarchy)
            InstantiatePrefeb(out rectCanvas);

        return rectCanvas;
    }

    public CanvasGroup PrefebDisplay()
    {
        CanvasGroup canvasGroup = GetPrefeb();

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.transform.position = rectTransPrefeb.transform.position;
        canvasGroup.transform.position += Vector3.down * offset;
        canvasGroup.alpha = 0;

        canvasGroup.AlphaLerp(1, 0.1f, 0.01f, null);
        canvasGroup.transform.MoveLerp(rectTransPrefeb.transform.position, 0.15f, 0.1f, null);

        return canvasGroup;
    }
    public void PrefebDisappear(CanvasGroup canvasGroup)
    {
        canvasGroup.transform.MoveLerp(rectTransPrefeb.transform.position + Vector3.up * offset, 0.15f, 0.1f, null);
        canvasGroup.AlphaLerp(0, 0.15f, 0.01f,
            () => canvasGroup.gameObject.SetActive(false));
    }
}
