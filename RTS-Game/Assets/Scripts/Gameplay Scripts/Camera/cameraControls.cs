using UnityEngine;

public class cameraControls : MonoBehaviour {

    public static GameObject cam;
    public int speed = 3;
    public int orbitingSpeed = 1;
    public int speedBonus = 2;
    public int buildRotationSpeed = 1;
    bool speedingUp = false;

    public static Vector2 mousePos;
    //private Vector3 camPos;
    public double maxZoomLimit;
    public double lowZoomLimit;

    public bool borderSlide = false;
    bool holdingCtrl = false;

    //public GameObject rotationObject;
    public static GameObject controlObject;

    //public EventType scrl = EventType.ScrollWheel;

    public void Start()
    {
        cam = GameObject.Find("MainCamera");
        controlObject = GameObject.Find("ControlObject");
    }

	private void Update() //Controls
    {
        if (Input.GetKey(KeyCode.LeftShift)) //Speed up with shift
        {
            if (!speedingUp)
            {
                speedingUp = true;
                speed = speed * speedBonus;
            } 
        } else
        {
            if (speedingUp)
            {
                speedingUp = false;
                speed = speed / speedBonus;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            holdingCtrl = true;
        } else
        {
            holdingCtrl = false;
        }

        if (Input.GetKey(KeyCode.W)) //Key controls
        {
            moveCamera("upward", 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveCamera("down", 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveCamera("left", 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveCamera("right", 1);
        }

        if (borderSlide) //Mouse border movement
        {
            if (Input.mousePresent)
            {
                mousePos = Input.mousePosition;

                if (mousePos.y < Screen.height / 12) 
                {
                    moveCamera("down", 1);
                }
                if (mousePos.y > Screen.height - Screen.height / 12)
                {
                    moveCamera("upward", 1);
                }
                if (mousePos.x < Screen.width / 12)
                {
                    moveCamera("left", 1);
                }
                if (mousePos.x > Screen.width - Screen.width / 12)
                {
                    moveCamera("right", 1);
                }
            }
        }

        if (holdingCtrl) //Building rotation on ctrl hold
        {
            if (buildHandler.exists)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)  //Scroll wheel rotation
                {
                    buildHandler.RotateBuilding(1 * buildRotationSpeed);
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    buildHandler.RotateBuilding(-1 * buildRotationSpeed);
                }
            }
        } else //Else zoom
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)  //Scroll wheel zoom
            {
                if (cam.transform.position.y > lowZoomLimit)
                {
                    moveCamera("z_in", 3);
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (cam.transform.position.y < maxZoomLimit)
                {
                    moveCamera("z_out", 3);
                }
            }
        }

        if (Input.GetMouseButton(1)) //Scroll wheel controls (Maybe swap for unit select?)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            moveCamera("r_right", mouseX * 300);
            //moveCamera("left", mouseX);
            //UnityEngine.Debug.Log(cam.transform.eulerAngles.x);
            if(mouseY > 0)
            {
                if (cam.transform.eulerAngles.x > 30f) //Bounds
                {
                    moveCamera("r_upward", mouseY * 200);
                }
            } else
            {
                if (cam.transform.eulerAngles.x < 60f)
                {
                    moveCamera("r_upward", mouseY * 200);
                }
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow)) //Arrow key orbit commands
        {
            moveCamera("r_right", 10);
            moveCamera("left", 2);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveCamera("r_left", 10);
            moveCamera("right", 2);
        }
        if (Input.GetKey(KeyCode.UpArrow)) //Arrow key rotation (up)
        {
            //Debug.Log(cam.transform.localRotation.x);
            if(cam.transform.localRotation.x > 0.2f)
            {
                moveCamera("r_upward", 0.8f);
            }
            //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y + (1 * Time.deltaTime), cam.transform.localPosition.z); "The introduction effect"
        }
        if (Input.GetKey(KeyCode.DownArrow)) //Arrow key rotation (down)
        {
            //Debug.Log(cam.transform.localRotation.x);
            if (cam.transform.localRotation.x < 0.6f)
            {
                moveCamera("r_down", 0.8f);
            }
        }
    }

    public void moveCamera(string dir, float argSpeed)
    {
        //controlObject.transform.eulerAngles = new Vector3(0, cam.transform.rotation.y, 0);

        if(dir == "upward") //Move the camera in one of the four directions
        {
            controlObject.transform.Translate(0, 0, speed * Time.deltaTime * (cam.transform.localPosition.y / 6));
            //cam.GetComponent<Transform>().Translate(0 * speed * Time.deltaTime, 1 * speed * Time.deltaTime, 1 * speed * Time.deltaTime);
            //camPos = new Vector3(camPos.x + speed * Time.deltaTime, camPos.y, camPos.z + speed * Time.deltaTime);
        }
        if (dir == "down")
        {
            controlObject.transform.Translate(0, 0, -speed * Time.deltaTime * (cam.transform.localPosition.y / 6));
            //cam.GetComponent<Transform>().Translate(0 * speed * Time.deltaTime, -1 * speed * Time.deltaTime, -1 * speed * Time.deltaTime);
            //camPos = new Vector3(camPos.x - speed * Time.deltaTime, camPos.y, camPos.z - speed * Time.deltaTime);
        }
        if (dir == "left")
        {
            controlObject.transform.Translate(-speed * Time.deltaTime * (cam.transform.localPosition.y / 6), 0, 0);
            //cam.GetComponent<Transform>().Translate((argSpeed * -2) * speed * Time.deltaTime, 0 * speed * Time.deltaTime, 0 * speed * Time.deltaTime);
            //camPos = new Vector3(camPos.x - speed * Time.deltaTime, camPos.y, camPos.z + speed * Time.deltaTime);
        }
        if (dir == "right")
        {
            controlObject.transform.Translate(speed * Time.deltaTime * (cam.transform.localPosition.y / 6), 0, 0);
            //cam.GetComponent<Transform>().Translate((argSpeed * 2) * speed * Time.deltaTime, 0 * speed * Time.deltaTime, 0 * speed * Time.deltaTime);
            //camPos = new Vector3(camPos.x + speed * Time.deltaTime, camPos.y, camPos.z - speed * Time.deltaTime);
        }

        if(dir == "z_in") //If zooming in, lerp towards mouse position
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 lw_mousePos = hit.point;
                controlObject.transform.position = Vector3.Lerp(controlObject.transform.position, lw_mousePos, speed * Time.deltaTime * 0.4f);
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - speed * Time.deltaTime * argSpeed * 5, cam.transform.position.z);
            }
        } else if (dir == "z_out")
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 lw_mousePos = hit.point;
                Vector3 diff = new Vector3(lw_mousePos.x - controlObject.transform.position.x, lw_mousePos.y - controlObject.transform.position.y, lw_mousePos.z - controlObject.transform.position.z);
                controlObject.transform.position = Vector3.Lerp(controlObject.transform.position, new Vector3(controlObject.transform.position.x - diff.x, controlObject.transform.position.y - diff.y, controlObject.transform.position.z - diff.z), speed * Time.deltaTime * 0.4f);
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + speed * Time.deltaTime * argSpeed * 5, cam.transform.position.z);
            }
        }
        /*else if (dir == "z_in") //Zoom
        {
            //cam.transform.Translate(0, 0, 8 * speed * Time.deltaTime);
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - speed * Time.deltaTime * argSpeed, cam.transform.position.z);
            controlObject.transform.Translate(0, 0, (1f / Mathf.Tan(cam.transform.rotation.x)) * speed * Time.deltaTime * argSpeed);
            //camPos = new Vector3(camPos.x + 3 * speed * Time.deltaTime, camPos.y - 6 * speed * Time.deltaTime, camPos.z + 3 * speed * Time.deltaTime);
            //rotationObject.transform.localPosition = new Vector3(0, cam.transform.localPosition.y / 4, cam.transform.localPosition.y - cam.transform.localPosition.y * 0.95f);
        }*/
        /*if ()
        {
            //cam.transform.Translate(0, 0, -8 * speed * Time.deltaTime);
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + speed * Time.deltaTime * argSpeed * 2, cam.transform.position.z);
            //controlObject.transform.Translate(0,0, (Mathf.Tan(cam.transform.rotation.x)) * speed * Time.deltaTime * -2 * argSpeed);
            //camPos = new Vector3(camPos.x - 3 * speed * Time.deltaTime, camPos.y + 6 * speed * Time.deltaTime, camPos.z - 3 * speed * Time.deltaTime);
            //rotationObject.transform.localPosition = new Vector3(0, cam.transform.localPosition.y / 4, cam.transform.localPosition.y - cam.transform.localPosition.y * 0.95f);
        }*/
        
        if(dir == "r_right") //Rotation (orbiting)
        {
            controlObject.transform.Rotate(0, argSpeed * Time.deltaTime, 0);
            //controlObject.GetComponent<Transform>().RotateAround(rotationObject.GetComponent<Transform>().position,new Vector3(0, 1, 0), argSpeed * orbitingSpeed * Time.deltaTime);
        }
        if(dir == "r_left")
        {
            controlObject.transform.Rotate(0, -argSpeed * Time.deltaTime, 0);
            //controlObject.GetComponent<Transform>().RotateAround(rotationObject.GetComponent<Transform>().position, new Vector3(0, -1, 0), argSpeed * orbitingSpeed * Time.deltaTime);
        }

        if(dir == "r_upward") //Rotation (up/down) //Ok gude
        {
            cam.transform.Rotate(-argSpeed * Time.deltaTime, 0, 0);
            //cam.GetComponent<Transform>().RotateAround(rotationObject.GetComponent<Transform>().position, new Vector3(1, 0, -1), 15 * speed * Time.deltaTime);
        }
        if (dir == "r_down")
        {
            cam.transform.Rotate(argSpeed * Time.deltaTime, 0, 0);
            //cam.GetComponent<Transform>().RotateAround(rotationObject.GetComponent<Transform>().position, new Vector3(-1, 0, 1), 15 * speed * Time.deltaTime);
        }

        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, controlObject.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        cam.transform.position = new Vector3(controlObject.transform.position.x, cam.transform.position.y, controlObject.transform.position.z);
        mapScripts.UpdateCameraPosition(controlObject.transform);
    }
}
