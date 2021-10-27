using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject operateGO;

    public void Start()
    {
        button ??= GetComponent<Button>();
        button.onClick.AddListener(() => operateGO.SetActive(!operateGO.activeInHierarchy));
    }
}
