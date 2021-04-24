using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool AlmostEquals(this double double1, double double2, double precision = 0.0000001)
    {
        return (Math.Abs(double1 - double2) <= precision);
    }

    public static bool AlmostEquals(this float double1, float double2, float precision = 0.0000001f)
    {
        return (Math.Abs(double1 - double2) <= precision);
    }

    public static string RemoveSymbolCharacters(this string str)
    {
        if (str == null)
            return null;
        char[] arr = str.ToCharArray();

        arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c)
                                          || char.IsWhiteSpace(c)
                                          || c == '-'
                                          || c == '('
                                          || c == ')'
                                          || c == '_')));
        return new string(arr);
    }

    public static bool EqualsIgnoreCase(this string s1, string s2)
    {
        return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string ContentsToString<T>(this T[] array)
    {
        var s = "{";
        foreach (var o in array)
            s += o + ",\n";
        s += "}";
        return s;
    }

    public static float RandomInclusive(this Vector2 v)
    {
        return UnityEngine.Random.Range(v.x, v.y);
    }

    public static Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 XZ(this Vector2 v, float y = 0)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector3 XY(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
}