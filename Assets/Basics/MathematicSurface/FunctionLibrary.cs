using UnityEngine;
using static UnityEngine.Mathf;
public static class FunctionLibrary
{
    public static float Wave(float x, float t)
    {
        return Sin(PI * (x + t));
    }

    public static float MultiWave(float x, float t)
    {
        return Sin(PI * (x + t));
    }
}