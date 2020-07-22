using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    int draggingFingerId = -1;

    float[] touchTimes = new float[10];
    Vector2[] touchMovements = new Vector2[10];
    public float definiteTapTimeThreshold;
    public float tapMovementMax;

    Vector3 mousePos;

    void Update()
    {
        if (!PlatformDetection.Instance.onMobile) //PC controls
        {
            Debug.Log("?");
            if (Input.GetMouseButtonDown(0))
            {
                touchTimes[0] = Time.unscaledTime;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 deltaPos = Input.mousePosition - mousePos;
                CameraController.Instance.cameraMovement += deltaPos;
                touchMovements[0] += new Vector2(Mathf.Abs(deltaPos.x), Mathf.Abs(deltaPos.y));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                float tapTime = Time.unscaledTime - touchTimes[0];
                Vector2 touchMovement = touchMovements[0];

                if (tapTime < definiteTapTimeThreshold)
                {
                    Tap(tapTime, touchMovement);
                }
                else if (touchMovement.x < tapMovementMax && touchMovement.y < tapMovementMax)
                {
                    Tap(tapTime, touchMovement);
                }
                else
                {
                    CameraController.Instance.lastCameraMovement = Input.mousePosition - mousePos;
                }

                touchTimes[0] = 0f;
                touchMovements[0] = Vector2.zero;
            }
            else
            {
                CameraController.Instance.cameraMovement = Vector2.zero;

                touchMovements[0] = Vector2.zero;
                touchTimes[0] = 0f;
            }

            mousePos = Input.mousePosition;
        }
        else //Mobile controls
        {
            int touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                //Limit the number of recorded touches to 10
                for (int i = 0; i < 10; i++)
                {
                    if (i == touchCount)
                    {
                        break;
                    }


                    Touch t = Input.touches[i];
                    int touchIndex = t.fingerId;

                    switch (t.phase)
                    {
                        case TouchPhase.Began:
                            touchTimes[touchIndex] = Time.unscaledTime;
                            break;
                        case TouchPhase.Moved:
                            Vector2 deltaPos = t.deltaPosition;

                        A:
                            if (draggingFingerId == -1)
                            {
                                draggingFingerId = touchIndex;
                                goto A;
                            }
                            else
                            {
                                CameraController.Instance.cameraMovement += deltaPos;
                            }

                            touchMovements[touchIndex] += new Vector2(Mathf.Abs(deltaPos.x), Mathf.Abs(deltaPos.y));

                            break;
                        case TouchPhase.Ended:
                            float tapTime = Time.unscaledTime - touchTimes[touchIndex];
                            Vector2 touchMovement = touchMovements[touchIndex];

                            if (tapTime < definiteTapTimeThreshold)
                            {
                                Tap(tapTime, touchMovement);

                                if (t.fingerId == draggingFingerId)
                                {
                                    draggingFingerId = -1;
                                }
                            }
                            else if (touchMovement.x < tapMovementMax && touchMovement.y < tapMovementMax)
                            {
                                Tap(tapTime, touchMovement);

                                if (t.fingerId == draggingFingerId)
                                {
                                    draggingFingerId = -1;
                                }
                            }
                            else
                            {
                                draggingFingerId = -1;
                                CameraController.Instance.lastCameraMovement = t.deltaPosition;
                            }

                            touchTimes[touchIndex] = 0f;
                            touchMovements[touchIndex] = Vector2.zero;
                            break;
                    }
                }

            }
            else
            {
                draggingFingerId = -1;
                CameraController.Instance.cameraMovement = Vector2.zero;

                int length = touchTimes.Length;
                for (int i = 0; i < length; i++)
                {
                    touchMovements[i] = Vector2.zero;
                    touchTimes[i] = 0f;
                }
            }
        }
    }

    public void Tap(float time, Vector2 movement)
    {
        //Debug.Log(time + " : " + movement);
        Knife.Instance.Cut();
    }
}
