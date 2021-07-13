using UnityEngine;
using static UnityEngine.Mathf;
public static class FunctionLibrary
{
    public enum FunctionName { Wave, MultiWave, Ripple }
    static Function[] functions = { Wave, MultiWave, Ripple };
    public delegate float Function(float x, float z, float t);

    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }

    public static float MultiWave(float x, float z, float t)
    {
        float y = Sin(PI * (x + z + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (x + z + t));
        return y * (2f / 3f);
    }

    public static float Ripple(float x, float z, float t)
    {
        float d = Abs(Sqrt(x * x + z * z));
        // float y = Sin(PI * d + t);
        // return y;
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10f * d);
    }
}