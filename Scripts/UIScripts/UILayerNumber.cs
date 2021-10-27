using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayerNumber : MonoBehaviour
{
    [SerializeField] private UIBoxController boxController;
    [SerializeField] private UICreateController createController;
    [SerializeField] private Text outputText;

    public void Start()
    {
        outputText ??= GetComponent<Text>();
    }
    public void Update()
    {
        float dis = outputText.transform.position.x - boxController.Position.x;
        outputText.text = ((int)(dis / createController.GetNoteScale().x * 0.25f)).ToString();
    }
}
