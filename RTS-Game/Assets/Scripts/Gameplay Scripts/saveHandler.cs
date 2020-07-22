using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class saveHandler : MonoBehaviour {

    public static string gameSaves = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\gameFiles";

    public static void OnGenerationSave(Texture2D lMap, Texture2D hMap, string saveName, int mapHeight)
    {
        if(!File.Exists(gameSaves + "\\" + saveName))
        {
            //Debug.Log("doesn't exist");
            Directory.CreateDirectory(gameSaves + "\\" + saveName);
            //File.Create(saveName).Dispose();
        }

        terrainGeneration.SaveImage(lMap, gameSaves + "\\" + saveName + "\\lMap.png");
        terrainGeneration.SaveImage(hMap, gameSaves + "\\" + saveName + "\\hMap.png");

        System.IO.File.WriteAllText(gameSaves + "\\" + saveName + "\\mapDetails.txt", "MapSize:" + (lMap.width - 1) / 512 +  "MapHeight:" + mapHeight);

        System.IO.File.WriteAllText(gameSaves + "\\" + saveName + "\\playerDetails.txt", "");
    }

    public static void SaveGame()
    {

    }

    public static SaveFile LoadSave(string saveName) //Make sure this file always exists. Otherwise it will crash.
    {
        string saveFile = gameSaves + "\\" + saveName;
        //Read the map details
        string mapDetails = "";
        string[] natureObjects = System.IO.File.ReadAllLines(saveFile + "\\nature.txt");

        byte[] dataStream;
        dataStream = File.ReadAllBytes(saveFile + "\\mapDetails.txt");

        mapDetails = System.Text.Encoding.UTF8.GetString(dataStream);

        int mapSize = (int)char.GetNumericValue(mapDetails[8]) * 512 + 1;
        
        //dataStream = File.ReadAllLines();

        //Load up the maps
        Texture2D lMap = new Texture2D(mapSize, mapSize);
        Texture2D hMap = new Texture2D(mapSize, mapSize);

        dataStream = File.ReadAllBytes(saveFile + "\\lMap.png");
        lMap.LoadImage(dataStream);

        dataStream = File.ReadAllBytes(saveFile + "\\hMap.png");
        hMap.LoadImage(dataStream);


        //Load up playerDetails
        dataStream = File.ReadAllBytes(saveFile +  "\\playerDetails.txt");
        string playerDetails = System.Text.Encoding.UTF8.GetString(dataStream);

        return new SaveFile(lMap, hMap, playerDetails, mapDetails, natureObjects);
    }
}

public class SaveFile
{
    public Texture2D lMap { get; set; }
    public Texture2D hMap { get; set; }
    public string playerDetails { get; set; }
    public string mapDetails { get; set; }
    public string[] natureObjects { get; set; }

    public SaveFile(Texture2D l_lMap, Texture2D l_hMap, string l_playerDetails, string l_mapDetails, string[] l_natureObjects)
    {
        lMap = l_lMap;
        hMap = l_hMap;
        playerDetails = l_playerDetails;
        mapDetails = l_mapDetails;
        natureObjects = l_natureObjects;
    }
}
