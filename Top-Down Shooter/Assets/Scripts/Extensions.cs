using UnityEngine;

public static class Extensions
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }

    public static float Remap(this float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    public static Quaternion LookAt(Vector3 pos, Vector3 target)
    {
        Vector3 dir = target - pos;
        return Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward) * Quaternion.Euler(0, 0, 270);
    }

    public static int GetIndexFromCoords(int x, int y, int xSize)
    {
        return y * xSize + x;
    }

    public static Vector2Int GetCoordsFromIndex(int index, int xSize, int ySize)
    {
        int y = Mathf.FloorToInt(index / xSize);
        int x = index - y * xSize;

        return new Vector2Int(x,y);
    }
}