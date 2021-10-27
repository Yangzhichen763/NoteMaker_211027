using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEase
{
    public static float PI = Mathf.PI;
    public static float EaseOutSine(float x)
        => Mathf.Sin((x * PI) * 0.5f);
    public static float EaseOutQuad(float x)
    {
        float xOffset = 1 - x;
        return 1 - xOffset * xOffset;
    }
    public static float EaseOutCubic(float x)
    {
        float xOffset = 1 - x;
        return 1 - xOffset * xOffset * xOffset;
    }
    public static float EaseOutQuart(float x)
    {
        float xOffset = 1 - x;
        return 1 - xOffset * xOffset * xOffset * xOffset;
    }

    public static float EaseInOutSine(float x)
        => -(Mathf.Cos(PI * x) - 1) * 0.5f;
    public static float EaseInOutQuad(float x)
    {
        float xOffset = -2 * x + 2;
        return x < 0.5f ? 2 * x * x : 1 - xOffset * xOffset * 0.5f;
    }
    public static float EaseInOutCubic(float x)
    {
        float xOffset = -2 * x + 2;
        return x < 0.5f ? 4 * x * x * x : 1 - xOffset * xOffset * xOffset * 0.5f;
    }
    public static float EaseInOutQuart(float x)
    {
        float xOffset = -2 * x + 2;
        return x < 0.5f ? 8 * x * x * x * x : 1 - xOffset * xOffset * xOffset * xOffset * 0.5f;
    }
}
