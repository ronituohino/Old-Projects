using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine.AI;

public class mapGeneration : MonoBehaviour {
    
    static int l_mapSize;
    public GameObject terrain;
    public GameObject terrainListGameObject;

    [Header("Load Options")]

    public string saveName;

    [Header("Generation Options")]

    public bool generateNewMap;

    [Space]

    public string generatedSaveName;

    [Space]

    public int mapSize;
    public int mapHeight;

    [Space]

    public string generatedHeightMapName;
    public string generatedLandMapName;
    //public string generatedBiomeMapName;

    //public int mapScale;

    //public static string path;
    //public static string path2;

    //public static string path3;

    [Space]

    public bool isIslandMap;

    public int islandAmount;

    [Range(0f,0.1f)]
    public float islandRandomChange;

    [Range(0.14f, 0.5f)]
    public float islandSize;

    [Space]

    public bool allowIslandCutoff;

    public enum EdgeGenerationTypes { random, edgeSplit, simple }
    public EdgeGenerationTypes edgeGenType;

    public int splitsPerEdge;

    public int islandShapeChanging;

    [Space]

    [Range(0.01f, 0.49f)]
    public float islandXClumpness;

    [Range(0.01f, 0.49f)]
    public float islandYClumpness;

    [Space]

    public bool populateMapWithObjects;

    [Space]

    public int lakeAmount;

    [Range(0.1f, 5f)]
    public float lakeSize;

    [Range(0f, 0.1f)]
    public float lakeRandomChange;

    [Range(0,100)]
    public int chanceForLakeExpansion; //we don't use this atm, idk what I should do with it xd

    [Space]

    public int iceIslandAmount;

    [Range(0.14f, 0.5f)]
    public float iceIslandSize;

    [Range(0f, 0.1f)]
    public float iceIslandRandomChange;

    [Space]

    public int randomIslandAmount;

    [Range(0.14f, 0.5f)]
    public float randomIslandSize;

    [Range(0f, 1f)]
    public float randomIslandRandomChange;

    [Space]

    public int mountains;

    [Range(0,100)]
    public int mountainCliffRatio; //Larger Value = More Cliffs

    public int maxCliffLength;
    //public static int maxCliffLength;

    [Range(0, 100)]
    public int cliffWaviness;

    [Range(0.001f, 0.01f)]
    public float mountainFallOffValue;

    public int mountainTopWidth;
    [Range(0.001f,0.01f)]
    public float mountainTopFallOffValue;

    [Space]

    public static Texture2D hMap;
    public static Texture2D lMap;

    public static int finalScale;
    public static int mapFSize;

    public static int mapH;

    [Header("Gameplay Settings")]

    public GameObject startingObject;

    public GameObject[] forestObjects;
    public GameObject[] desertObjects;
    public GameObject[] snowObjects;

    private void Start()
    {
        //UnityEngine.Debug.Log("heyehy2");
        float totalLoadTime = 0;
        if (generateNewMap)
        {
            //Generate a new heightmap
            if(generatedHeightMapName != "" && generatedLandMapName != "")
            {
                finalScale = mapSize * 512 + 1; //We need to read the map size during loading, reading dataStream length isn't reliable,
                                                //Maybe save the map size on a separate save file?
                mapFSize = mapSize * 200;
                mapH = mapHeight;

                lMap = new Texture2D(finalScale, finalScale); //landMap
                hMap = new Texture2D(finalScale, finalScale); //heightMap

                Color32 black = new Color32(0, 0, 0, 255);
                for(int x = 0; x < hMap.width; x++)
                {
                    for(int y = 0; y < hMap.height; y++)
                    {
                        hMap.SetPixel(x, y, black);
                    }
                }
                /*int finalScale = mapSize * 512 + 1;
                Texture2D lMap = new Texture2D(finalScale, finalScale); //landMap
                Texture2D hMap = new Texture2D(finalScale, finalScale); //heightMap

                mapScripts.PassMapScale(finalScale);*/

                //path = Environment.CurrentDirectory + "\\Assets\\Textures\\Height Maps\\" + generatedHeightMapName + ".png";
                //path2 = Environment.CurrentDirectory + "\\Assets\\Textures\\Height Maps\\" + generatedLandMapName + ".png";
                //path3 = Environment.CurrentDirectory + "\\Assets\\Textures\\Height Maps\\" + generatedBiomeMapName + ".png";

                Stopwatch sw = new Stopwatch();

                //Generate biomeMap
                sw.Start();
                lMap = terrainGeneration.GenerateLandMap(lMap ,isIslandMap, islandAmount, islandYClumpness, islandSize, 
                    islandShapeChanging, lakeAmount, lakeSize, islandXClumpness, mapSize, edgeGenType.ToString(), 
                    splitsPerEdge, allowIslandCutoff, chanceForLakeExpansion, iceIslandAmount, 
                    iceIslandSize, randomIslandAmount, randomIslandSize, islandRandomChange, iceIslandRandomChange, randomIslandRandomChange);

                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: generating lMap + bMap, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();


                //Generate shores
                sw.Start();
                terrainGeneration.GenerateShores(lMap, hMap);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: generating shores to hMap, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                //Generate heightMap
                sw.Start();
                hMap = terrainGeneration.GenerateImageHeightMap(lMap, hMap, mapSize, mountains, maxCliffLength, mountainCliffRatio, mountainTopWidth, mountainTopFallOffValue, mountainFallOffValue, cliffWaviness);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: generating hMap, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                //Fade biomes together
                sw.Start();
                lMap = terrainGeneration.BiomeFade(lMap);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: fading biomes, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                //Generate a map (load a heightMap texture into terrain)
                sw.Start();
                GenTerrains(hMap);
                sw.Stop();
                UnityEngine.Debug.Log("Stage: loading heightMap to terrain + calculating NavMesh, Seconds:" + sw.Elapsed.Seconds);
                totalLoadTime += sw.Elapsed.Seconds;
                sw.Reset();

                //Assign textures to the terrain
                sw.Start();
                assignSplatmap.AssignMap(terrainListGameObject, lMap, true, mapSize);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: assigning colors to terrain, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                if (populateMapWithObjects)
                {
                    sw.Start();
                    objectPopulation.Populate(lMap, hMap, forestObjects, desertObjects, snowObjects, mapHeight, generatedSaveName);
                    sw.Stop();
                    totalLoadTime += sw.Elapsed.Seconds;
                    UnityEngine.Debug.Log("Stage: populating map with objects, Seconds: " + sw.Elapsed.Seconds);
                    sw.Reset();
                }
                

                mapScripts.LoadMap(lMap);

                //Vector2[] spawnableLand = new Vector2[terrainGeneration.mainLand.Count + terrainGeneration.snowLand.Count + terrainGeneration.randomLand.Count];
                sw.Start();
                empireManager.StartEmpire(hMap, mapHeight, startingObject, GameObject.Find("Buildings"));
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: setting up an empire, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                sw.Start();
                saveHandler.OnGenerationSave(lMap, hMap, generatedSaveName, mapHeight);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: saving world as a profile, Seconds: " + sw.Elapsed.Seconds);
                sw.Reset();

                UnityEngine.Debug.Log("Total load time: " + totalLoadTime);
            } else
            {
                UnityEngine.Debug.Log("Can't generate maps without names!");
            }
        } else
        {
            if(Directory.Exists(saveHandler.gameSaves + "\\" + saveName)) //Check if this saveFile exists
            {
                Stopwatch sw = new Stopwatch();

                sw.Start();
                //UnityEngine.Debug.Log(saveName);
                SaveFile sf = saveHandler.LoadSave(saveName);

                string c = sf.mapDetails[19].ToString();
                string c2 = sf.mapDetails[20].ToString();

                //UnityEngine.Debug.Log(String.Concat(c, c2));
                mapH = int.Parse(String.Concat(c, c2));

                finalScale = sf.lMap.width;
                mapSize = ((finalScale - 1) / 512);
                mapFSize = mapSize * 200;

                lMap = new Texture2D(finalScale, finalScale); //landMap
                hMap = new Texture2D(finalScale, finalScale); //heightMap

                lMap = sf.lMap;
                hMap = sf.hMap;
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: loading data from memory, Seconds: " + sw.Elapsed.Seconds);

                sw.Start();
                GenTerrains(hMap);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: loading data to terrain + calculating NavMesh, Seconds: " + sw.Elapsed.Seconds);

                sw.Start();
                //Assign textures to the terrain
                assignSplatmap.AssignMap(terrainListGameObject, lMap, true, mapSize);
                sw.Stop();
                totalLoadTime += sw.Elapsed.Seconds;
                UnityEngine.Debug.Log("Stage: assigning colors to terrain, Seconds: " + sw.Elapsed.Seconds);

                if (populateMapWithObjects)
                {
                    sw.Start();
                    objectPopulation.LoadNaturalObjects(sf.natureObjects, forestObjects, desertObjects, snowObjects);
                    sw.Stop();
                    totalLoadTime += sw.Elapsed.Seconds;
                    UnityEngine.Debug.Log("Stage: loading natural objects, Seconds: " + sw.Elapsed.Seconds);
                }
                

                UnityEngine.Debug.Log("Total load time: " + totalLoadTime);
                mapScripts.LoadMap(lMap);
            } else
            {
                UnityEngine.Debug.Log("Save file doesn't exist!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        mapScripts.PassMapScale(mapFSize);
    }

    public void GenTerrains(Texture2D hMap)
    {
        //l_mapSize = mapSize;
        GameObject[] terrainList = new GameObject[mapSize * mapSize]; //GameObject list of the terrains
        
        //UnityEngine.Debug.Log(mapSize * mapSize);
        for (int i = 0; i < mapSize * mapSize; i++) //Instantiate empty terrains
        {
            terrainList[i] = Instantiate(terrain, GetTerrainPos(i), Quaternion.identity, terrainListGameObject.transform) as GameObject;
            //for(int y = 0; y < mapSize; y++)
            //{

            //}
        }

        Terrain[,] terrainlistTerrains = new Terrain[mapSize, mapSize];
        int rev = 0;
        for(int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                terrainlistTerrains[x, y] = terrainList[rev].GetComponent<Terrain>();
                rev++;
            }
        }
        
        NavMeshSurface nvs;
        //Stopwatch stopwatch = new Stopwatch();

        
        foreach(GameObject terrain in terrainList) //Set terrainData for terrains
        {
            Terrain t = terrain.GetComponent<Terrain>();
            t.terrainData = new TerrainData();
            TerrainData td = t.terrainData;

            td.heightmapResolution = 513;
            //if(mapHeight != null)
            //{
            td.size = new Vector3(200, mapH, 200);
            //} else
            //{
                //td.size = new Vector3(200, 10, 200);
            //}
            

            LoadTerrain(td, hMap, t);

            //stopwatch.Start();

            terrain.GetComponent<TerrainCollider>().terrainData = td;
            nvs = terrain.AddComponent<NavMeshSurface>();
            UnityEngine.Debug.Log(nvs.agentTypeID);
            nvs.BuildNavMesh();

            //stopwatch.Stop();
            //UnityEngine.Debug.Log(stopwatch.Elapsed.Seconds);

            t.heightmapPixelError = 0;
            t.detailObjectDistance = 250;
        }
        

        for(int i = 0; i < mapSize * mapSize; i++) //Set neighbouring terrains
        {
            //for(int y = 0; y < mapSize; y++)
            //{
            int comp = i;

            int x = 0;
            int y = 0;

            while (comp > mapSize - 1)
            {
                comp -= mapSize;
                x++;
            }
            y = comp;

            Terrain selected = terrainlistTerrains[x,y];

            Terrain top = new Terrain();
            Terrain right = new Terrain();
            Terrain bottom = new Terrain();
            Terrain left = new Terrain();

                
            try
            {
                top = terrainlistTerrains[x, y + 1];
            } catch(IndexOutOfRangeException)
            {
                top = null;
            }     
                

            try
            {
                right = terrainlistTerrains[x + 1, y];
            }
            catch (IndexOutOfRangeException)
            {
                right = null;
            }

            try
            {
                bottom = terrainlistTerrains[x,y - 1];
            }
            catch (IndexOutOfRangeException)
            {
                bottom = null;
            }

            try
            {
                left = terrainlistTerrains[x - 1,y];
            }
            catch (IndexOutOfRangeException)
            {
                left = null;
            }

            selected.SetNeighbors(left, top, right, bottom);
            //}
        }

        /*for (int x = 0; x < mapSize; x++) //Calculating NavMesh
        {
            for (int y = 0; y < mapSize; y++)
            {
                terrainList[x, y].GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }*/
    }

    public void LoadTerrain(TerrainData tTerrain, Texture2D hMap, Terrain curTerrain) //Load a .png image to a terrain
    {
        //Color32 temp = new Color32();
        //Texture2D text = new Texture2D(1, 1);
        //text.SetPixel(0, 0, new Color32(200,200,200,255));
        //UnityEngine.Debug.Log(text.GetPixel(0, 0).r + text.GetPixel(0, 0).g);

        int h = tTerrain.heightmapResolution;
        int w = tTerrain.heightmapResolution;

        int yStart = (int)curTerrain.gameObject.transform.position.z / 200;
        int xStart = (int)curTerrain.gameObject.transform.position.x / 200;

        if (yStart >= 1)
        {
            yStart *= 512;
        }
        if (xStart >= 1)
        {
            xStart *= 512;
        }

        int yEnd = yStart + 512;
        int xEnd = xStart + 512;

        //Debug.Log(yStart);
        //Debug.Log(yEnd);

        float[,] data = new float[h, w];
        data = tTerrain.GetHeights(0, 0, w, h);
        int xData = 0;
        int yData = 0;
        for (int y = yStart; y < yEnd + 1; y++)
        {
            for (int x = xStart; x < xEnd + 1; x++)
            {
                try
                {
                    data[yData, xData] = hMap.GetPixel(x, y).grayscale;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
                xData++;
            }
            xData = 0;
            yData++;
        }

        tTerrain.SetHeights(0, 0, data);
    }

    private Vector3 GetTerrainPos(int y)
    {
        int x = 0;
        while(y > mapSize - 1)
        {
            y -= mapSize;
            x++;
        }
        return new Vector3(x * 200, 0, y * 200);
    }
}
