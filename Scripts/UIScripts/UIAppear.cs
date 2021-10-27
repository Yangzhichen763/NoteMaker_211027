using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAppear : MonoBehaviour
{
    public static UIAppear manager;

    [SerializeField] private CanvasGroup blackPanel;

    public void Awake()
    {
        manager = this;
    }
    public void Start()
    {
        if (!blackPanel.gameObject.activeInHierarchy)
            blackPanel.gameObject.SetActive(true);
        blackPanel.alpha = 1;

        Appear(true);
    }
    public void Appear(bool appear)
    {
        if (appear)
            blackPanel.AlphaLerp(0, 0.15f, 0.01f, () => blackPanel.gameObject.SetActive(false));
        else
        {
            blackPanel.gameObject.SetActive(true);
            blackPanel.AlphaLerp(1, 0.2f, 0.01f, () =>
            {
                Quit();
            });
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
