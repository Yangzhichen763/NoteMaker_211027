using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameInputManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UIAppear.manager.Appear(false);
    }
}
