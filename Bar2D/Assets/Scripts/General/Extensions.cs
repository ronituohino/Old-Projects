using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
    public static Vector2 ToV2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static float Remap(this float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    //Returns rigidbody torque to rotate object to targetAngle
    public static float CalculateTorque(float targetAngle, Transform transform, float rotationMultiplier, Vector2 rotationClamp)
    {
        if(transform != null)
        {
            float diff = Mathf.Clamp(Vector2.SignedAngle(transform.up, Vector2.up) - targetAngle, rotationClamp.x, rotationClamp.y);
            return diff * Mathf.Deg2Rad * rotationMultiplier;
        }
        else
        {
            Debug.LogWarning("Transform missing!");
            return 0f;
        }
    }

    public static bool RoundEqualsFloat(float a, float b, int power)
    {
        float mult = Mathf.Pow(10f, power);

        return Mathf.RoundToInt(a * mult) / mult == Mathf.RoundToInt(b * mult) / mult;
    }


    public static bool RoundEqualsVector(Vector2 a, Vector2 b, int power)
    {
        float mult = Mathf.Pow(10f, power);

        return Mathf.RoundToInt(a.x * mult) / mult == Mathf.RoundToInt(b.x * mult) / mult
            && Mathf.RoundToInt(a.y * mult) / mult == Mathf.RoundToInt(b.y * mult) / mult;
    }

    public static void SetAllValues<T>(this T[] array, T value, int len = -1)
    {
        if(len == -1)
        {
            len = array.Length;
        }

        for(int i = 0; i < len; i++)
        {
            array[i] = value;
        }
    }

    public static void SetTreeToLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        int childCount = children.Length;

        for(int i = 1; i < childCount; i++)
        {
            GameObject g = children[i].gameObject;
            g.layer = layer;
            SetTreeToLayer(g, layer);
        }
    }

    public static int WrapAroundRange(int i, int min, int max)
    {
        if (max <= 0)
        {
            return 0;
        }

        int div = i / max;

        if (div > 0)
        {
            return i % max + min;
        }
        else
        {
            return i;
        }
    }

    public static IEnumerator AnimationWait(Animator animator, string animationStateName, Action function)
    {
        // Wait until we enter the current state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
        {
            yield return null;
        }

        // Now, Wait until the current state is done playing
        while ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 0.99f)
        {
            yield return null;
        }

        // Done playing. Do something below!
        function.Invoke();
    }

    public static IEnumerator ConditionWait(Func<bool> condition, Action function)
    {
        while(!condition.Invoke())
        {
            yield return null;
        }

        function.Invoke();
    }

    public static float CalculateDropTime(float height)
    {
        return Mathf.Sqrt((2f * height) / Physics2D.gravity.magnitude);
    }

    public static float LookAt(Vector2 obj, Vector2 target)
    {
        return Vector2.SignedAngle(Vector2.up, target - obj);
    }

    public static RaycastHit2D[] FindSurroundingObjectsOnLayers(Vector2 origin, int[] layerIndex, float range)
    {
        float total = 0;
        foreach(int i in layerIndex)
        {
            total += Mathf.Pow(2, layerIndex[i]);
        }
        return Physics2D.RaycastAll(origin, Vector2.zero, range, Mathf.RoundToInt(total));
    }
}