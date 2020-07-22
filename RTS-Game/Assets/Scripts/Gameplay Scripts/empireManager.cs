using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class empireManager : MonoBehaviour {

    //public GameObject startingObject;
    static System.Random r = new System.Random();

    [Header("Person clothing colouring")]

    public GameObject personObject;

    [Space]

    public Material[] skinColors;
    public Material[] torsoColors;
    public Material[] legColors;
    public Material[] feetColors;

    public static GameObject person;

    public static Material[] sCol;
    public static Material[] tCol;
    public static Material[] lCol;
    public static Material[] fCol;

    private void Start()
    {
        sCol = skinColors;
        tCol = torsoColors;
        lCol = legColors;
        fCol = feetColors;

        person = personObject;
    }
    public static void StartEmpire(Texture2D hMap , int mapHeight , GameObject startingObject, GameObject buildingsParent)
    {
        List<Vector2> points = new List<Vector2>();
        float grayScale;

        for(int x = 0; x < hMap.width; x++)
        {
            for(int y = 0; y < hMap.height; y++)
            {
                grayScale = hMap.GetPixel(x, y).grayscale;
                if (grayScale > 0 && grayScale < 0.4f)
                {
                    points.Add(new Vector2(x, y));
                }
            }
        }

        Vector2 startPoint = points[r.Next(0, points.Count-1)];

        startingObject = Instantiate(startingObject, new Vector3(startPoint.x * 0.389f, hMap.GetPixel((int)startPoint.x, (int)startPoint.y).grayscale * mapHeight, startPoint.y * 0.389f), Quaternion.Euler(-90, 0, 0), buildingsParent.transform);
        startingObject.AddComponent<townScripts>();
        startingObject.AddComponent<peopleControlHandler>();
        townScripts.startTown(startingObject, 2, tCol, fCol, lCol, sCol, person);
        //Debug.Log(startPoint);

        GameObject cam = GameObject.Find("MainCamera");
        GameObject controlObject = GameObject.Find("ControlObject");

        controlObject.transform.position = new Vector3(startPoint.x * 0.389f - 10, 0, startPoint.y * 0.389f - 10);
        cam.transform.position = new Vector3(startPoint.x * 0.389f - 10, 20, startPoint.y * 0.389f - 10);

        points.Clear();
    }
}
