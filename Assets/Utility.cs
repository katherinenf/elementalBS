using UnityEngine;
using UnityEngine.UI;

public static class Utility
{
    public static T ChangeAlpha<T>(this T g, float newAlpha)
        where T : Graphic
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
        return g;
    }
}