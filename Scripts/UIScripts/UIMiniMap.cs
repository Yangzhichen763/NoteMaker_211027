using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMiniMap : MonoBehaviour
{
    [SerializeField] private UICreateController mainCreator, minimapCreator;

    public void OnEnable()
    {
        mainCreator.changeData += SetMinimapNote;
    }
    public void OnDisable()
    {
        mainCreator.changeData -= SetMinimapNote;
    }
    /// <summary> miniMap µÄ¸úËæÔË¶¯ </summary>
    /// <param name="line"></param>
    /// <param name="column"></param>
    /// <param name="inputIndex"></param>
    private void SetMinimapNote(int line,int column,int inputIndex)
    {
        switch (inputIndex)
        {
            case 0:
                minimapCreator.InputMouseDown(line, column);
                break;
            case 1:
                minimapCreator.InputMouse(line, column);
                break;
            case 2:
                minimapCreator.InputRemoveMouse(line, column);
                break;
        }
    }
}
