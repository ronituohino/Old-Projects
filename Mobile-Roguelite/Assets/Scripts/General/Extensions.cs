using System;
using System.Collections;
using UnityEngine;

public static class Extensions
{
    public static Vector2 ToV2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static float LookAt(Vector2 obj, Vector2 target)
    {
        return Vector2.SignedAngle(Vector2.up, target - obj);
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

    public static float ToCm(this float pixelsMoved)
    {
        return (pixelsMoved / Screen.dpi) * 2.54f;
    }

    public static bool IsDrag(this float pixelsMoved)
    {
        return pixelsMoved.ToCm() >= InputManager.Instance.dragThresholdInCm;
    }


    public static Vector2 SmoothStep(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(Mathf.SmoothStep(a.x, b.x, t), Mathf.SmoothStep(a.y, b.y, t));
    }


    public static void TranslateRect(RectTransform rect, Vector2 startPosition, Vector2 endPosition, float lerp)
    {
        Vector2 screenPoint = SmoothStep
                    (
                        startPosition,
                        endPosition,
                        lerp
                    );

        Vector2 localPoint = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                    (
                        InputManager.Instance.sceneCanvas,
                        screenPoint,
                        InputManager.Instance.sceneCamera,
                        out localPoint
                    );

        rect.anchoredPosition = localPoint;
    }

    public static void TranslateRect(RectTransform rect, Vector2 endPosition, ref Vector2 velocity, float smoothTime)
    {
        Vector2 screenPoint = Vector2.SmoothDamp
                   (
                       RectTransformUtility.WorldToScreenPoint(InputManager.Instance.sceneCamera, rect.position),
                       endPosition,
                       ref velocity,
                       smoothTime
                   );

        Vector2 localPoint = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                    (
                        InputManager.Instance.sceneCanvas,
                        screenPoint,
                        InputManager.Instance.sceneCamera,
                        out localPoint
                    );

        rect.anchoredPosition = localPoint;
    }
}