using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimeManager
{
    public static float timeScale = 1;

    public static float fixedDeltaTime => Time.fixedDeltaTime * timeScale;
}
