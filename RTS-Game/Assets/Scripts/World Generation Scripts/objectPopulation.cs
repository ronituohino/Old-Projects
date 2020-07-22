using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;

public class objectPopulation : MonoBehaviour {

    static int finalScale;

    static List<Vector2> forestPoints = new List<Vector2>();
    static List<Vector2> desertPoints = new List<Vector2>();
    static List<Vector2> snowPoints = new List<Vector2>();

    static List<Vector2> selectedPoints = new List<Vector2>();

    static System.Random r = new System.Random();

    //static GameObject[] forestObj;
    //static GameObject[] snowObj;
    //static GameObject[] desertObj;

    static GameObject loadedNatureObjectsParent;

    static GameObject go;
    /*static GameObject forestParent;
    static GameObject desertParent;
    static GameObject snowParent;*/

    static float mapHeight;

    static List<string> natureObjects = new List<string>();

    public static void Populate(Texture2D lMap, Texture2D hMap, GameObject[] forestObj, GameObject[] desertObj, GameObject[] snowObj, float mapH, string genSaveName)
    {
        Color32 yellow = new Color32(255,255,0,255);
        Color32 white = new Color32(255, 255, 255, 255);
        Color32 green = new Color32(0, 255, 0, 255);
        Color32 blue = new Color32(0, 0, 255, 255);

        Color32 temp = new Color32();

        mapHeight = mapH;

        //Initialize natural object arrays
        loadedNatureObjectsParent = GameObject.Find("NaturalObjects");

        //forestObj = new GameObject[forestObj.Length];
        //forestObj = forestObj;
        //forestParent = GameObject.Find("forestObj");

        //desertObj = new GameObject[desertObj.Length];
        //desertObj = desertObj;
        //desertParent = GameObject.Find("desertObj");

        //snowObj = new GameObject[snowObj.Length];
        //snowObj = snowObj;
        //snowParent = GameObject.Find("snowObj");

        finalScale = lMap.width;
        for(int x = 0; x < finalScale; x++)
        {
            for(int y = 0; y < finalScale; y++)
            {
                temp = lMap.GetPixel(x, y);
                if (temp.Equals(yellow))
                {
                    desertPoints.Add(new Vector2(x, y));
                } else if (temp.Equals(white))
                {
                    snowPoints.Add(new Vector2(x, y));
                } else if (temp.Equals(green))
                {
                    forestPoints.Add(new Vector2(x, y));
                }
            }
        }

        

        int tempVal;
        for(int x = 0; x < finalScale; x += r.Next(10,30))
        {
            for(int y = 0; y < finalScale; y += r.Next(10,20))
            {
                tempVal = r.Next(-15, 15);
                x += tempVal;

                temp = lMap.GetPixel(x, y);
                if (temp.Equals(yellow) || temp.Equals(green) || temp.Equals(white))
                {
                    PlaceObject(new Vector2(x, y), temp, yellow, white, green, lMap, hMap, forestObj, snowObj, desertObj);
                }
                
                x -= tempVal;
            }
        }

        System.IO.File.WriteAllLines(saveHandler.gameSaves + "\\" + genSaveName + "\\nature.txt", natureObjects.ToArray());
    }

    static void PlaceObject(Vector2 point, Color32 temp, Color32 yellow, Color32 white, Color32 green, Texture2D lMap, Texture2D hMap, GameObject[] fObj, GameObject[] sObj, GameObject[] dObj) //Takes a Vector2 and turns it into a Vector3 and places an object there
    {
        Vector3 pointInWorld = new Vector3(CheckObjectOverFlow(point.x) * 0.389f, mapHeight * hMap.GetPixel((int)point.x,(int)point.y).grayscale, CheckObjectOverFlow(point.y) * 0.389f); //0.389 is the relation of 513 to 200 (1 terrain scale in image / 1 terrain scale in map)

        if(temp.Equals(yellow))
        {
            go = Instantiate(dObj[r.Next(0, dObj.Length)], pointInWorld, Quaternion.Euler(-90, r.Next(0, 360), 0), loadedNatureObjectsParent.transform) as GameObject; //Add some random rotation
            //float randomVal = r.Next(90, 110) / 100f;
            //go.transform.localScale = new Vector3(randomVal, randomVal, randomVal);

            //textBlock = textBlock.Insert(textBlock.Length, go.transform.rotation.ToString() + " " + randomVal);
        } else if(temp.Equals(white))
        {
            go = Instantiate(sObj[r.Next(0, sObj.Length)], pointInWorld, Quaternion.Euler(-90, r.Next(0,360), 0), loadedNatureObjectsParent.transform) as GameObject; //Add some random rotation
            //float randomVal = r.Next(90, 110) / 100f;
            //go.transform.localScale = new Vector3(randomVal, randomVal, randomVal);

            //textBlock = textBlock.Insert(textBlock.Length, go.transform.rotation.ToString() + " " + randomVal);
        } else if(temp.Equals(green))
        {
            go = Instantiate(fObj[r.Next(0, fObj.Length)], pointInWorld, Quaternion.Euler(-90, r.Next(0,360),0), loadedNatureObjectsParent.transform) as GameObject; //Add some random rotation
        }

        float randomVal = r.Next(90, 110) / 100f;
        go.transform.localScale = new Vector3(randomVal, randomVal, randomVal);

        //textBlock = ;
        
        natureObjects.Add(Math.Round(pointInWorld.x, 2).ToString() + ":" + Math.Round(pointInWorld.y, 2).ToString() + ":" + Math.Round(pointInWorld.z, 2).ToString() + ":" + go.transform.eulerAngles.x.ToString() + ":" + go.transform.eulerAngles.y.ToString() + ":" + go.transform.eulerAngles.z.ToString() + ":" + randomVal + ":" + go.name.Replace("(Clone)", ""));
    }

    private static float CheckObjectOverFlow(float value)
    {
        if(value >= finalScale)
        {
            return value - finalScale;
        }
        else if(value < 0)
        {
            return finalScale + value;
        } else
        {
            return value;
        }
    }



    public static void LoadNaturalObjects(string[] objects, GameObject[] forestObj, GameObject[] desertObj, GameObject[] snowObj) //Instantiate objects from text file
    {
        loadedNatureObjectsParent = GameObject.Find("NaturalObjects");

        //forestObj = forestObj;
        //desertObj = desertObj;
        //snowObj = snowObj;

        for(int i = 0; i < objects.Length; i++) //Foreach line in text file
        {
            string curObj = objects[i];
            string[] stringCoords = curObj.Split(':'); //Split coordinates into parts

            float[] coords = new float[stringCoords.Length - 1]; //Parse strings into floats
            for(int c = 0; c < stringCoords.Length - 1; c++)
            {
                coords[c] = float.Parse(stringCoords[c]);
            }

            GameObject go = Instantiate(FindNatureObject(stringCoords[7], forestObj, snowObj, desertObj), new Vector3(coords[0], coords[1], coords[2]), Quaternion.Euler(coords[3], coords[4], coords[5]), loadedNatureObjectsParent.transform) as GameObject; //Instantiate the object
            go.transform.localScale = new Vector3(coords[6], coords[6], coords[6]);
        }
    }

    private static GameObject FindNatureObject(string natureObject, GameObject[] fObj, GameObject[] sObj, GameObject[] dObj)
    {
        for(int i = 0; i < fObj.Length; i++)
        {
            if (fObj[i].name.Equals(natureObject))
            {
                return fObj[i];
            }
        }
        for (int i = 0; i < dObj.Length; i++)
        {
            if (dObj[i].name.Equals(natureObject))
            {
                return dObj[i];
            }
        }
        for (int i = 0; i < sObj.Length; i++)
        {
            if (sObj[i].name.Equals(natureObject))
            {
                return sObj[i];
            }
        }

        UnityEngine.Debug.Log("Couldn't read object from txt file!");
        return null; //This should not ever be called
    } 





    /*static void SetArea(string biome, Texture2D lMap, Texture2D hMap)
    {
        Vector2 startPoint;
        if (biome == "forest")
        {
            if (forestPoints.Count > 0)
            {
                startPoint = forestPoints[r.Next(0, forestPoints.Count - 1)];
                PopulateArea(startPoint, lMap, hMap);
            }
        }
        else if (biome == "desert")
        {
            if (desertPoints.Count > 0)
            {
                startPoint = desertPoints[r.Next(0, desertPoints.Count - 1)];
                PopulateArea(startPoint, lMap, hMap);
            }
        }
        else if (biome == "snow")
        {
            if (snowPoints.Count > 0)
            {
                startPoint = snowPoints[r.Next(0, snowPoints.Count - 1)];
                PopulateArea(startPoint, lMap, hMap);
            }
        }
    }

    static void PopulateArea(Vector2 startPoint, Texture2D lMap, Texture2D hMap)
    {
        int heightAdd = 0;
        int sway = 0;

        Color32 yellow = new Color32(255, 255, 0, 255);
        Color32 white = new Color32(255, 255, 255, 255);
        Color32 green = new Color32(0, 255, 0, 255);

        Color32 temp = new Color32();

        for (int i = 0; i < 25; i++) //UP
        {
            heightAdd += r.Next(4, 8);
            sway += r.Next(4, 8);
            switch (r.Next(0, 2))
            {
                case 0:
                    PlaceObject(new Vector2(startPoint.x + r.Next(0, sway), startPoint.y + heightAdd), temp, yellow, white, green, lMap, hMap); //More right
                    break;
                case 1:
                    PlaceObject(new Vector2(startPoint.x + (r.Next(0, sway) * -1), startPoint.y + heightAdd), temp, yellow, white, green, lMap, hMap); //More left
                    break;
            }
        }

        heightAdd = 0;
        sway = 0;

        for (int i = 0; i < 25; i++) //RIGHT
        {
            heightAdd += r.Next(4, 8);
            sway += r.Next(4, 8);
            switch (r.Next(0, 2))
            {
                case 0:
                    PlaceObject(new Vector2(startPoint.x + sway, startPoint.y + r.Next(0, heightAdd)), temp, yellow, white, green, lMap, hMap); //More up
                    break;
                case 1:
                    PlaceObject(new Vector2(startPoint.x + sway, startPoint.y + (r.Next(0, heightAdd) * -1)), temp, yellow, white, green, lMap, hMap); //More down
                    break;
            }
        }

        heightAdd = 0;
        sway = 0;

        for (int i = 0; i < 25; i++) //DOWN
        {
            heightAdd += r.Next(4, 8);
            sway += r.Next(4, 8);
            switch (r.Next(0, 2))
            {
                case 0:
                    PlaceObject(new Vector2(startPoint.x + r.Next(0, sway), startPoint.y - heightAdd), temp, yellow, white, green, lMap, hMap);
                    break;
                case 1:
                    PlaceObject(new Vector2(startPoint.x + (r.Next(0, sway) * -1), startPoint.y - heightAdd), temp, yellow, white, green, lMap, hMap);
                    break;
            }
        }

        heightAdd = 0;
        sway = 0;

        for (int i = 0; i < 25; i++) //LEFT
        {
            heightAdd += r.Next(4, 8);
            sway += r.Next(4, 8);
            switch (r.Next(0, 2))
            {
                case 0:
                    PlaceObject(new Vector2(startPoint.x - sway, startPoint.y + r.Next(0, heightAdd)), temp, yellow, white, green, lMap, hMap); //More up
                    break;
                case 1:
                    PlaceObject(new Vector2(startPoint.x - sway, startPoint.y + (r.Next(0, heightAdd) * -1)), temp, yellow, white, green, lMap, hMap); //More down
                    break;
            }
        }
    }*/
}
