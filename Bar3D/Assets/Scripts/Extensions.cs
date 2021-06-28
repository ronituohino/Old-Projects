using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all sorts of functions used throughout this project
public static class Extensions
{
    public static float Remap(this float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    public static Quaternion RotationBetween(Vector3 a, Vector3 b)
    {
        Quaternion q;

        Vector3 cross = Vector3.Cross(a, b);

        q.x = cross.x;
        q.y = cross.y;
        q.z = cross.z;

        q.w = Mathf.Sqrt(Mathf.Pow(a.magnitude, 2f) * Mathf.Pow(b.magnitude, 2f)) + Vector3.Dot(a, b);

        return q.normalized;
    }

    public static void Populate<T>(this T[] array, T value)
    {
        int len = array.Length;
        for (int i = 0; i < len; i++)
        {
            array[i] = value;
        }
    }

    public static int WrapAroundRange(int i, int start, int min, int max)
    {
        if (max <= 0)
        {
            return 0;
        }

        int total = i + start;
        int div = total / max;

        if (div > 0)
        {
            return total % max + min;
        }
        else
        {
            return total;
        }
    }
}