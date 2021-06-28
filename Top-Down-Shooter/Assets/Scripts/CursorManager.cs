using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    //Disable calls to UpdateCursor
    public bool lockCursorTexture = false;

    public Texture2D normalCursor;

    [Space]

    public Texture2D sideScaleCursor;
    public Texture2D sideScaleCursorRotated;

    [Space]

    public Texture2D cornerScaleCursor;
    public Texture2D cornerScaleCursorRotated;

    private void Awake()
    {
        UpdateCursor(CursorStyle.Normal, false);
    }

    public enum CursorStyle
    {
        Normal,
        SideScale,
        CornerScale
    }

    public void UpdateCursor(CursorStyle style, bool rotated)
    {
        if(!lockCursorTexture)
        {
            switch (style)
            {
                case CursorStyle.Normal:
                    Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorStyle.SideScale:
                    if (rotated)
                    {
                        Cursor.SetCursor(sideScaleCursorRotated, new Vector2(16, 16), CursorMode.Auto);
                    }
                    else
                    {
                        Cursor.SetCursor(sideScaleCursor, new Vector2(16, 16), CursorMode.Auto);
                    }
                    break;
                case CursorStyle.CornerScale:
                    if (rotated)
                    {
                        Cursor.SetCursor(cornerScaleCursorRotated, new Vector2(16, 16), CursorMode.Auto);
                    }
                    else
                    {
                        Cursor.SetCursor(cornerScaleCursor, new Vector2(16, 16), CursorMode.Auto);
                    }
                    break;
            }
        }
    }
}