using UnityEngine;

public static class Utilities
{
    public static float Lerp(float x, float y, float value)
    {
        return x * (1 - value) + y * value;
    }
}
