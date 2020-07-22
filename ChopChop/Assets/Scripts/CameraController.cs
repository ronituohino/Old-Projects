using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public bool cuttingMode = true;

    public float cameraMoveThreshold;
    public float cameraFallThreshold;
    public float swipeThereshold;

    [Space]

    public AnimationCurve cameraCurve;
    public float cameraFallSpeed = 0.01f;

    [Space]

    public float cameraSensitivity;

    public Animator cameraAnimator;

    //InputManages.cs writes to these values
    [HideInInspector]
    public Vector2 cameraMovement = Vector2.zero;
    [HideInInspector]
    public Vector2 lastCameraMovement = Vector2.zero;

    bool fallToOtherSide = false;
    bool slideCam = false;

    bool resetLastCameraMovement = false;

    float cameraSlideValue = 0f;

    bool calledSwitchMode = false;

    void Update()
    {
        if (lastCameraMovement != Vector2.zero)
        {
            resetLastCameraMovement = true;
        }

        if (cameraMovement != Vector2.zero && !slideCam)
        {
            Knife.Instance.ableToCut = false;

            if (lastCameraMovement.x / Screen.width > swipeThereshold)
            {
                fallToOtherSide = true;
            }

            float relativeX = cameraMovement.x / Screen.width;
            //float relativeY = cameraMovement.y / Screen.height;
            float absX = Mathf.Abs(relativeX);

            if (cuttingMode)
            {
                if (relativeX > 0)
                {
                    if (absX > cameraMoveThreshold)
                    {
                        cameraSlideValue = Mathf.Clamp01(absX * cameraSensitivity);
                        cameraAnimator.Play("Camera", 0, cameraSlideValue);

                        if (cameraSlideValue > cameraFallThreshold)
                        {
                            fallToOtherSide = true;
                        }
                        else
                        {
                            fallToOtherSide = false;
                        }
                    }
                }
            }
            else
            {
                if (relativeX < 0)
                {
                    if (absX > cameraMoveThreshold)
                    {
                        cameraSlideValue = Mathf.Clamp01(1 - absX * cameraSensitivity);
                        cameraAnimator.Play("Camera", 0, cameraSlideValue);

                        if (cameraSlideValue < cameraFallThreshold)
                        {
                            fallToOtherSide = true;
                        }
                        else
                        {
                            fallToOtherSide = false;
                        }
                    }
                }
            }
        }
        else if ((cameraSlideValue > 0 && cameraSlideValue < 1) || fallToOtherSide) //Fall the camera to one side or the other
        {
            slideCam = true;

            if (cuttingMode)
            {
                if (fallToOtherSide)
                {
                    FallLeft();
                }
                else
                {
                    FallRight();
                }
            }
            else
            {
                if (fallToOtherSide)
                {
                    FallRight();
                }
                else
                {
                    FallLeft();
                }
            }

        }

        if (resetLastCameraMovement)
        {
            lastCameraMovement = Vector2.zero;
            resetLastCameraMovement = false;
        }
    }

    void FallLeft()
    {
        cameraSlideValue += cameraFallSpeed;

        if (cameraSlideValue > 1f)
        {
            slideCam = false;
            fallToOtherSide = false;
            cameraSlideValue = 1f;
            cuttingMode = false;

            calledSwitchMode = false;

            cameraMovement = Vector2.zero;
            lastCameraMovement = Vector2.zero;
        }

        cameraAnimator.Play("Camera", 0, cameraCurve.Evaluate(cameraSlideValue));

        if (!calledSwitchMode)
        {
            if (cuttingMode)
            {
                SwitchedMode(false); 
            }
        }
    }

    void FallRight()
    {
        cameraSlideValue -= cameraFallSpeed;

        if (cameraSlideValue < 0f)
        {
            slideCam = false;
            fallToOtherSide = false;
            cameraSlideValue = 0f;
            cuttingMode = true;

            calledSwitchMode = false;

            cameraMovement = Vector2.zero;
            lastCameraMovement = Vector2.zero;

            Knife.Instance.ableToCut = true;
        }

        cameraAnimator.Play("Camera", 0, cameraCurve.Evaluate(cameraSlideValue));

        if (!calledSwitchMode)
        {
            if (!cuttingMode)
            {
                SwitchedMode(true);
            }
        }
    }

    //Called if the mode is switched
    void SwitchedMode(bool toCuttingMode)
    {
        calledSwitchMode = true;

        if (!toCuttingMode)
        {
            BowlHandler.Instance.GiveBowl();
        }
    }
}
