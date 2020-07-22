using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Drawing;
using System.IO;
using System;
using System.Diagnostics;

public class terrainGeneration : MonoBehaviour
{
    static System.Random r = new System.Random();
    static int finalScale;

    private static Vector2 movementPoint = new Vector2();
    static List<Vector2> finalMountainLocations = new List<Vector2>(); //Store the actual and final locations of mountains
    static List<Vector2> finalMountainLocations2 = new List<Vector2>();

    static List<Vector2> islandPoints = new List<Vector2>();

    public static List<Vector2> mainLand = new List<Vector2>();
    public static List<Vector2> snowLand = new List<Vector2>();
    public static List<Vector2> randomLand = new List<Vector2>();

    //Used in seaEdge generation
    static List<Vector2> seaEdges = new List<Vector2>();
    static List<Vector2> list = new List<Vector2>();

    //Ice
    //static List<Vector2> icePoints = new List<Vector2>();
    //static List<Vector2> snowLand = new List<Vector2>();

    //Sea shores
    static List<Vector2> seaEdges1 = new List<Vector2>();
    static List<Vector2> seaEdges2 = new List<Vector2>();
    static List<Vector2> seaEdges3 = new List<Vector2>();
    static List<Vector2> seaEdges4 = new List<Vector2>();

    static List<Vector2> TseaEdges = new List<Vector2>();
    static List<Vector2> TseaEdges1 = new List<Vector2>();
    static List<Vector2> TseaEdges2 = new List<Vector2>();
    static List<Vector2> TseaEdges3 = new List<Vector2>();
    static List<Vector2> TseaEdges4 = new List<Vector2>();

    //Used in our fill function
    static List<Vector2> drawPoints1 = new List<Vector2>();
    static List<Vector2> drawPoints2 = new List<Vector2>();

    static List<Vector2> finalPoints = new List<Vector2>();

    //Used in biome fade
    static List<Vector2> FDEdges = new List<Vector2>();
    static List<Vector2> FSEdges = new List<Vector2>();
    static List<Vector2> DSEdges = new List<Vector2>();

    static List<Vector2> DumpFDEdges = new List<Vector2>();
    static List<Vector2> DumpFSEdges = new List<Vector2>();
    static List<Vector2> DumpDSEdges = new List<Vector2>();

    static int splitsPerEdgeS;
    static int edgeDirInt;
    static int edgeDirChanged;
    static int lastTimeDirChanged;
    static bool splitting;

    static int mountainTopWidth;
    static float mountainFallOffValue;
    static float mountainTopFallOffValue;

    //Used fkin everywhere
    static bool x1plus;
    static bool y1plus;

    static bool x1minus;
    static bool y1minus;

    //Used in shoreGen
    static float vXPlus;
    static float vYPlus;
    static float vXMinus;
    static float vYMinus;


    public static Texture2D GenerateLandMap(Texture2D lMap ,bool island, int islandAmount, float mapYC, 
        float islandScale2, int islandShapeChanging, int lakeAmount, float lakeSize, float mapXC, int scale, 
        string edgeGenType, int splitsPerEdge, bool allowIslandCutoff, int chanceForLakeExpansion, 
        int iceIslandAmount, float iceIslandSize, int randomIslandAmount, float randomIslandSize, float islandChange, 
        float iceIslandChange, float randomIslandChange)
    {
        finalScale = scale * 512 + 1;
        splitsPerEdgeS = splitsPerEdge;
        int idetail;

        //GENERATE ISLANDS
        Color32 blue = new Color32(0, 0, 255, 255);
        Color32 white = new Color32(255, 255, 255, 255);
        Color32 yellow = new Color32(255, 255, 0, 255);
        Color32 green = new Color32(0, 255, 0, 255);

        //List<Vector2> iceMList = new List<Vector2>();

        if (island) //Make an island map (Generate islands and sea is the base)
        {
            if(islandAmount == 0)
            {
                UnityEngine.Debug.Log("Can't generate an island map with no islands!");
                return lMap;
            }

            for (int x = 0; x < finalScale; x++) //Make the biome map base blue (sea)
            {
                for (int y = 0; y < finalScale; y++)
                {
                    lMap.SetPixel(x, y, blue);
                }
            }

            //int cutSize = (1 - 2 * mapXC) * finalScale / (islandAmount + 1);
            //int startSize = mapXC * finalScale;
            //Debug.Log(cutSize);

            float unitSize;
            int startDir;
            int dir;
            Vector2 drawingPoint = new Vector2(0,0);

            int idk;
            bool startMinus;
            bool cutOff;

            int whileLoopBreakPoint;

            //bool iceObstacle = false;
            bool ice = false;

            bool randomIsland = false;

            for (int i = 0; i < islandAmount + iceIslandAmount + randomIslandAmount; i++) //Make an island (improve this algorithm thank you very much)
            {
                if(i < islandAmount) //Regular island
                {
                    randomIsland = false;
                    ice = false;
                    
                } else if(i >= islandAmount + iceIslandAmount) //Random island
                {
                    randomIsland = true;
                    ice = false;
                }
                else //Ice island
                {
                    ice = true;
                    randomIsland = false;
                }

                if(!ice && !randomIsland)
                {
                    idetail = (int)(100 * (islandScale2 + UnityEngine.Random.Range(-islandChange, islandChange)));
                    unitSize = 10 * (islandScale2 + UnityEngine.Random.Range(-islandChange, islandChange));
                } else if(ice)
                {
                    /*switch (r.Next(0, 2)) //Ratio of ice obstacles to snowy lands
                    {
                        case 0:
                            iceObstacle = false;
                            break;
                        default:
                            iceObstacle = false; //Turn this into true later
                            break;
                    }*/
                    /*if (iceObstacle)
                    {
                        idetail = (int)(60 * (iceIslandSize + UnityEngine.Random.Range(-iceIslandChange, iceIslandChange)));
                        unitSize = 10 * (iceIslandSize + UnityEngine.Random.Range(-iceIslandChange, iceIslandChange));
                    } else
                    {*/
                        idetail = (int)(100 * (iceIslandSize + UnityEngine.Random.Range(-iceIslandChange, iceIslandChange)));
                        unitSize = 10 * (iceIslandSize + UnityEngine.Random.Range(-iceIslandChange, iceIslandChange));
                    //}
                } else
                {
                    idetail = (int)(100 * (randomIslandSize + UnityEngine.Random.Range(-randomIslandChange, randomIslandChange)));
                    unitSize = 10 * (randomIslandSize + UnityEngine.Random.Range(-randomIslandChange, randomIslandChange));
                }
                
                startDir = r.Next(0,8);

                if (i == 0)
                {
                    startDir = 0;
                } else if(i == islandAmount - 1)
                {
                    startDir = 4;
                }

                if (unitSize < 1f)
                {
                    unitSize = 1f;
                }

                //Starting point of the island
                //drawingPoint = new Vector2(0,0);

                whileLoopBreakPoint = 0; //Prevents crashing once again

                //Spawn island start points, (possible improvement; make it an option if islands can spawn ontop one another)
                /*if (islandStartPoints.Count > 0 && clumpedIslands)
                {
                    do
                    {
                        //that thing belongs here
                        whileLoopBreakPoint++;

                    } while (IslandSpawnAllowed(drawingPoint, islandStartPoints) && whileLoopBreakPoint < finalScale / 10 * 4);
                } else
                {
                    drawingPoint = new System.Drawing.Point((int)(r.Next((int)(((i) * cutSize)), (i + 1) * cutSize)), r.Next((int)(finalScale * (0.2f + islandClumpiness / 4)), (int)(finalScale * (0.8f - islandClumpiness / 4))));   //(int)bl.x, (int)tr.x - ((int)tr.x - (int)bl.x) / 2,   finalScale - (int)tr.y, finalScale - (int)bl.y
                }*/

                //Use these to check islandStart is going well
                //Debug.Log(((i * cutSize) + mapXC * (finalScale / 20)));
                //Debug.Log(((i + 1) * cutSize) + mapXC * (finalScale / 20));

                if(!ice && !randomIsland) //Regular island
                {
                    drawingPoint = new Vector2(r.Next((int)(finalScale * (mapXC)), (int)(finalScale * (1 - mapXC))), r.Next((int)(finalScale * (mapYC)), (int)(finalScale * (1 - mapYC))));   //(int)bl.x, (int)tr.x - ((int)tr.x - (int)bl.x) / 2,   finalScale - (int)tr.y, finalScale - (int)bl.y  |||| r.Next(((i * cutSize) + mapXC * (finalScale / 20)), (i + 1) * cutSize + mapXC * (finalScale / 20))
                } else if(ice) //Ice island
                {
                    switch (r.Next(0, 2))
                    {
                        case 0: //Up
                            drawingPoint = new Vector2(r.Next((int)(finalScale * (mapXC / 2)), (int)(finalScale * (1 - (mapXC / 2)))), r.Next((int)(finalScale * (1 - (mapYC / 2))), finalScale)); //Upper part
                            //startDir = 6;
                            break;
                        case 1: //Down
                            drawingPoint = new Vector2(r.Next((int)(finalScale * (mapXC / 2)), (int)(finalScale * (1 - (mapXC / 2)))), r.Next(0, (int)(finalScale * (mapYC / 2)))); //Lower part
                            //startDir = 2;
                            break;
                    }
                } else //Random island in the middle of nowhere
                {
                    switch (r.Next(0, 2)) //Make dis work
                    {
                        case 0: //Left
                            drawingPoint = new Vector2(r.Next(0, (int)(finalScale * (mapXC))), r.Next((int)(finalScale * mapYC), (int)(finalScale * (1 - mapYC)))); //Upper part
                            //startDir = 6;
                            break;
                        case 1: //Right
                            drawingPoint = new Vector2(r.Next((int)(finalScale * (1 - mapXC)), finalScale), r.Next((int)(finalScale * mapYC), (int)(finalScale * (1 - mapYC)))); //Lower part
                            //startDir = 2;
                            break;
                    }
                }

                dir = startDir;

                islandPoints.Add(drawingPoint);

                idk = 0;
                startMinus = false;
                cutOff = false;

                for (int rev2 = 0; rev2 < 8; rev2++) //Make the island points
                {
                    for (int rev = 0; rev < idetail; rev++)
                    {
                        //Add more ways of doing edges,
                        //1. Dir changes a speicific number of times on one main edge
                        //2. Dir changes randomly but is flipped back after a specified amount of island has been done
                        if(edgeGenType != "simple")
                        {
                            dir = CheckEdgeGenType(dir, edgeGenType, islandShapeChanging, rev2);
                        }
                        
                        idk++;
                        drawingPoint = MoveEdgePoint(drawingPoint, dir, unitSize);
                        islandPoints.Add(drawingPoint);

                        if(rev2 > 2) //Cutoff if we are close to the start of the island
                        {
                            if (IsClose(drawingPoint, islandPoints[0], idetail) && allowIslandCutoff) 
                            {
                                cutOff = true;
                                //Debug.Log(islandPoints[0]);
                                //Debug.Log(drawingPoint);
                                break;
                            }
                        }
                    }

                    if (cutOff && allowIslandCutoff)
                    {
                        break;
                    }

                    if(dir == 7) //If direction is started at for ex. 4 it is flipped to 0 once it has reached 7
                    {
                        startMinus = true;
                    } else
                    {
                        startMinus = false;
                    }
                    if (startMinus)
                    {
                        dir -= 7;
                    } else
                    {
                        dir++;
                    }
                }

                whileLoopBreakPoint = 0; //breakoutInt (In case something fucks up, prevents crashing)

                //FINISH ISLAND LAST EDGE
                if (!cutOff)
                {
                    while (drawingPoint.x != islandPoints[0].x && drawingPoint.y != islandPoints[0].y)
                    {
                        whileLoopBreakPoint++;
                        if (whileLoopBreakPoint > finalScale / 10)
                        {
                            break;
                        }
                        drawingPoint = FinishIsland(drawingPoint, islandPoints[0], Mathf.RoundToInt(unitSize), idetail);
                        if (drawingPoint.x != islandPoints[0].x && drawingPoint.y != islandPoints[0].y)
                        {
                            islandPoints.Add(drawingPoint);
                        }
                    }
                }
                
                

                //Draw the actual island
                if(!ice && !randomIsland) //Regular island 
                {
                    FillArea(islandPoints, lMap, yellow);
                } else if(randomIsland && !ice) //Random island
                {
                    FillArea(islandPoints, lMap, green);
                }
                else //Icy island
                {
                    FillArea(islandPoints, lMap, white);
                }
                
                lMap.Apply();

                //SaveImage(bMap, path);

                finalPoints.Clear();
                islandPoints.Clear(); //Reuse in lakeGen
            }
        }

        //Store all mainLand pixels
        //Color32 white = new Color32(255,255,255,255); //White nibba
        Color32 pixelColor;
        for (int x = 0; x < finalScale; x++)
        {
            for(int y = 0; y < finalScale; y++)
            {
                pixelColor = lMap.GetPixel(x, y);
                if (pixelColor.Equals(yellow))
                {
                    mainLand.Add(new Vector2(x, y));
                }
                if (pixelColor.Equals(white))
                {
                    snowLand.Add(new Vector2(x, y));
                }
                if (pixelColor.Equals(green))
                {
                    randomLand.Add(new Vector2(x, y));
                }
            }
        }

        //SaveImage(lMap, "testp.png");
        //GENERATE BIOMEMAP

        //Generate a biomeMap, this is a layout of the biomes. We then transfer them to lMap
        Texture2D bMap = new Texture2D(finalScale, finalScale); 
        Texture2D noiseMap = GenerateTexture();

        //SaveImage(noiseMap, Environment.CurrentDirectory + "\\Assets\\Textures\\Height Maps\\noiseMap.png");

        //PAINT BIOMEMAP
        //Color32 pix = new Color32();

        float edgeHeight = 0.55f; //Ratio of desert-forest. Lower == more desert, less woodlands

        int biomeEdgeLength = 50;

        for (int x = 0; x < noiseMap.width; x++)
        {
            for (int y = 0; y < noiseMap.height; y++)
            {
                //Set static edge colors
                if(x < biomeEdgeLength ||  x > finalScale - biomeEdgeLength)
                {
                    bMap.SetPixel(x, y, green);
                } else if (y < biomeEdgeLength || y > finalScale - biomeEdgeLength) {
                    bMap.SetPixel(x, y, green);
                } else {
                    if (noiseMap.GetPixel(x, y).grayscale > edgeHeight) //Desert-forest ratio
                    {
                        bMap.SetPixel(x, y, yellow);
                    }
                    else
                    {
                        bMap.SetPixel(x, y, green);
                    }
                }
            }
        }

        //We could save all the locations of the icy mountains previously and then just copy+paste
        //Make tundra
        //UnityEngine.Debug.Log(icePoints.Count);
        /*for(int i = 0; i < icePoints.Count; i++)
        {
            bMap.SetPixel((int)icePoints[i].x, (int)icePoints[i].y, white);
        }*/
        for (int i = 0; i < snowLand.Count; i++)
        {
            bMap.SetPixel((int)snowLand[i].x, (int)snowLand[i].y, white);
        }

        //Set biomes for landMap
        Vector2 pixelVector;
        for (int i = 0; i < mainLand.Count; i++)
        {
            pixelVector = new Vector2(mainLand[i].x, mainLand[i].y);
            lMap.SetPixel((int)pixelVector.x, (int)pixelVector.y, bMap.GetPixel((int)pixelVector.x, (int)pixelVector.y));
        }
        for (int i = 0; i < randomLand.Count; i++)
        {
            pixelVector = new Vector2(randomLand[i].x, randomLand[i].y);
            lMap.SetPixel((int)pixelVector.x, (int)pixelVector.y, bMap.GetPixel((int)pixelVector.x, (int)pixelVector.y));
        }

        //GENERATE LAKES
        if (lakeAmount > 0)
        {
            //List<Vector2> lakePoints = new List<Vector2>();
            bool cutOff = false;
            bool validSpawn = false;

            for (int i = 0; i < lakeAmount; i++) //For amount of lakes to be generated
            {
                Vector2 lakeStart = mainLand[r.Next(0, mainLand.Count - 1)];

                //Check lakeSpawn
                int whileCut = 0;
                validSpawn = CheckLakeSpawn(lakeStart, finalScale / 30, finalScale, lMap);
                while (validSpawn)
                {
                    whileCut++;
                    lakeStart = mainLand[r.Next(0, mainLand.Count-1)];

                    validSpawn = CheckLakeSpawn(lakeStart, finalScale / 10, finalScale, lMap);

                    if (whileCut > 10)
                    {
                        break;
                    }
                    //if CheckLakeSpawn returns true, make new lakeStart
                }
                //Debug.Log(whileCut);
                Vector2 lakeDrawPoint = lakeStart;

                islandPoints.Add(lakeStart);
                int lakeUSize = (int)(10 * lakeSize);
                //int lakeDetail = (int)(50 * lakeSize);

                //Make a lake
                for(int dir = 0; dir < 8; dir++)
                {
                    for (int l = 0; l < lakeSize; l++) //var = lakeDetail
                    {
                        lakeDrawPoint = MoveEdgePoint(lakeDrawPoint, dir, lakeUSize);
                        islandPoints.Add(lakeDrawPoint);

                        if(l > 3 && dir > 0)
                        {
                            if (IsClose(lakeDrawPoint, lakeStart, lakeUSize))
                            {
                                cutOff = true;
                                break;
                            }
                        }
                    }
                    if (cutOff)
                    {
                        break;
                    }
                }
                
                //Finish lake edges (maybe do this idk)
                /*if (!cutOff)
                {
                    whileCut = 0;
                    while (lakeDrawPoint.x != lakeStart.x || lakeDrawPoint.x != lakeStart.x)
                    {
                        lakeDrawPoint = FinishIsland(lakeDrawPoint, lakeStart, lakeSize, finalScale);
                        islandPoints.Add(lakeDrawPoint);

                        whileCut++;

                        if (whileCut > 300)
                        {
                            break;
                        }
                    }
                }*/

                //Draw the lake
                FillArea(islandPoints, lMap, blue);
                //Clear the list and start a new lake
                islandPoints.Clear();
            }
        }

        //Return the image
        lMap.Apply();
        return lMap;
    }

    public static Texture2D GenerateImageHeightMap(Texture2D lMap, Texture2D hMap, int scale, int mountains, int maxMountainLength, int mountainCliffRatio, int mountainTopWidth, float mountainTopFallOffValue, float mountainFallOffValue, int waviness)
    {
        if (mountains > 0)
        {
            GenerateMountains(mountains, maxMountainLength, mountainCliffRatio, mountainTopWidth, mountainTopFallOffValue, mountainFallOffValue, scale, hMap, waviness, lMap);
        }
        hMap.Apply();
        return hMap;
    }

    //mainLand TERRAIN GENERATION
    public static Texture2D GenerateShores(Texture2D lMap, Texture2D hMap) //Use biomeMap to generate terrain heights for the heightMap
    {
        //Start with the sea and lake edge smoothing

        Color32 green = new Color32(0, 255, 0, 255);
        Color32 blue = new Color32(0, 0, 255, 255);
        Color32 yellow = new Color32(255, 255, 0, 255);

        Color32 white = new Color32(255, 255, 255, 255);
        Color32 shoreCol = new Color32(255, 255, 0, 255);

        Color32 tempC = new Color32();
        Vector2 tempV;
        //Color32 pixel = new Color32();

        //Scan the bMap and mark all the sea edges
        for (int x = 0; x < finalScale; x++) 
        {
            for(int y = 0; y < finalScale; y++)
            {
                tempC = lMap.GetPixel(x, y);
                if (tempC.Equals(green) || tempC.Equals(yellow) || tempC.Equals(white)) 
                {
                    tempV = CheckOverflow(x + 1, y);
                    tempC = lMap.GetPixel((int)tempV.x, (int)tempV.y);
                    if (tempC.Equals(blue))
                    {
                        seaEdges.Add(tempV);
                    }

                    tempV = CheckOverflow(x - 1, y);
                    tempC = lMap.GetPixel((int)tempV.x, (int)tempV.y);
                    if (tempC.Equals(blue))
                    {
                        seaEdges.Add(tempV);
                    }

                    tempV = CheckOverflow(x, y + 1);
                    tempC = lMap.GetPixel((int)tempV.x, (int)tempV.y);
                    if (tempC.Equals(blue))
                    {
                        seaEdges.Add(tempV);
                    }

                    tempV = CheckOverflow(x, y - 1);
                    tempC = lMap.GetPixel((int)tempV.x, (int)tempV.y);
                    if (tempC.Equals(blue))
                    {
                        seaEdges.Add(tempV);
                    }
                }
            }
        }

        x1plus = false;
        x1minus = false;
        y1plus = false;
        y1minus = false;

        //GENERATE SEASHORES

        float addUp = 0f; 
        float addOn = 0.01f; //Shore startHeight
        bool rising = true;

        int jump = 1;
        int jumpLeft = 0;
        bool startUp = true;

        int rev = 0;

        //We need some variation to stuff
        while(true)
        {
            //Calculate the heightMap heightGrowth value (addUp)
            if(rev > 12) //Shore length
            {
                rising = false;
            }
            if (rising)
            {
                if (rev > 0 && jumpLeft == 0)
                {
                    jumpLeft = r.Next(10, 20);
                    addOn = UnityEngine.Random.Range(0.04f, 0.06f) / jumpLeft; //Shore heighGrowth
                }
            } else
            {
                //jumpLeft = r.Next(60, 300);
                addOn = UnityEngine.Random.Range(0.0001f, 0.0002f); //Shore heighGrowth
            }

            addUp += addOn;

            //Go apply that to all known edges
            for (int i = 0; i < 4; i++)
            {
                list.Clear();
                if (rev == 0 && startUp)
                {
                    startUp = false;
                    list = seaEdges;
                    ExpandShore(x1plus, y1plus, x1minus, y1minus, jump, lMap, hMap, addUp, rising, blue, white, shoreCol);
                    i--;
                }
                else if (i == 0)
                {
                    list = seaEdges1;
                    ExpandShore(x1plus, y1plus, x1minus, y1minus, jump, lMap, hMap, addUp, rising, blue, white, shoreCol);
                }
                else if(i == 1)
                {
                    list = seaEdges2;
                    ExpandShore(x1plus, y1plus, x1minus, y1minus, jump, lMap, hMap, addUp, rising, blue, white, shoreCol);
                }
                else if (i == 2)
                {
                    list = seaEdges3;
                    ExpandShore(x1plus, y1plus, x1minus, y1minus, jump, lMap, hMap, addUp, rising, blue, white, shoreCol);
                }
                else if (i == 3)
                {
                    list = seaEdges4;
                    ExpandShore(x1plus, y1plus, x1minus, y1minus, jump, lMap, hMap, addUp, rising, blue, white, shoreCol);
                }
            }

            seaEdges1.Clear(); //Duplicate that shit
            seaEdges2.Clear();
            seaEdges3.Clear();
            seaEdges4.Clear();

            seaEdges1 = new List<Vector2>(TseaEdges1);
            seaEdges2 = new List<Vector2>(TseaEdges2);
            seaEdges3 = new List<Vector2>(TseaEdges3);
            seaEdges4 = new List<Vector2>(TseaEdges4);

            TseaEdges1.Clear();
            TseaEdges2.Clear();
            TseaEdges3.Clear();
            TseaEdges4.Clear();

            if (jumpLeft > 0)
            {
                jumpLeft--;
            }
            if (seaEdges1.Count + seaEdges2.Count + seaEdges3.Count + seaEdges4.Count < 1)
            {
                break;
            }
            rev++;
        }

        seaEdges1.Clear(); //Clear out ram
        seaEdges2.Clear();
        seaEdges3.Clear();
        seaEdges4.Clear();

        TseaEdges1.Clear();
        TseaEdges2.Clear();
        TseaEdges3.Clear();
        TseaEdges4.Clear();

        lMap.Apply();
        hMap.Apply();

        return hMap;
    }
    //mainLand SHORE STUFFS
    private static void ExpandShore(bool xPlus, bool yPlus, bool xMinus, bool yMinus, int jump, Texture2D lMap, Texture2D hMap, float addUp, bool rising, Color32 seaColor, Color32 white, Color32 shoreCol)
    {
        Vector2 v;
        Vector2 temp;
        //int xTemp;
        //int yTemp;
        for (int i = 0; i < list.Count; i++) //Do one layer of the sea edge
        {
            v = list[i];

            vXPlus = v.x + jump;
            vYPlus = v.y + jump;
            vXMinus = v.x - jump;
            vYMinus = v.y - jump;

            /*xPlus = false; //Check conditions whether we are on the edge of the map
            yPlus = false;
            xMinus = false;
            yMinus = false;

            if (vXPlus < finalScale)
            {
                xPlus = true;
            }
            if (vYPlus < finalScale)
            {
                yPlus = true;
            }
            if (vXMinus > 0)
            {
                xMinus = true;
            }
            if (vYMinus > 0)
            {
                yMinus = true;
            }*/

            if(addUp >= 1)
            {
                addUp = 1f;
            }

            //if (xPlus)
            //{
            temp = CheckOverflow(vXPlus, v.y);
            if (NotPainted(temp.x, temp.y, hMap))
            {
                if (HasLand(lMap, temp.x, temp.y, seaColor))
                {
                    ShoreGen(temp.x, temp.y, hMap, lMap, addUp * 255, 1, rising, white, shoreCol);
                }
            }
            //}
            //if (xMinus)
            //{
            temp = CheckOverflow(vXMinus, v.y);
            if (NotPainted(temp.x, temp.y, hMap))
            {
                if (HasLand(lMap, temp.x, temp.y, seaColor))
                {
                    ShoreGen(temp.x, temp.y, hMap, lMap, addUp * 255, 2, rising, white, shoreCol);
                }
            }
            //}
            //if (yPlus)
            //{
            temp = CheckOverflow(v.x, vYPlus);
            if (NotPainted(temp.x, temp.y, hMap))
            {
                if (HasLand(lMap, temp.x, temp.y, seaColor))
                {
                    ShoreGen(temp.x, temp.y, hMap, lMap, addUp * 255, 3, rising, white, shoreCol);
                }
            }
            //}
            //if (yMinus)
            //{
            temp = CheckOverflow(v.x, vYMinus);
            if (NotPainted(temp.x, temp.y, hMap))
            {
                if (HasLand(lMap, temp.x, temp.y, seaColor))
                {
                    ShoreGen(temp.x, temp.y, hMap, lMap, addUp * 255, 4, rising, white, shoreCol);
                }
            }
            //}
        }
    }
    //mainLand SHORE STUFFS
    private static void ShoreGen(float x, float y, Texture2D hMap, Texture2D lMap, float newBrightness, int listNum, bool rising, Color32 white, Color32 shoreCol)
    {
        //Color32 white = new Color32(255, 255, 255, 255);
        hMap.SetPixel((int)x, (int)y, new Color32((byte)(newBrightness), (byte)(newBrightness), (byte)(newBrightness), 255));

        if (rising && lMap.GetPixel((int)x,(int)y) != white)
        {
            lMap.SetPixel((int)x, (int)y, shoreCol); //Make shore sandy on the lMap
        }
        

        if(listNum == 1)
        {
            TseaEdges1.Add(new Vector2(x, y));
        }
        else if(listNum == 2)
        {
            TseaEdges2.Add(new Vector2(x, y));
        }
        else if (listNum == 3)
        {
            TseaEdges3.Add(new Vector2(x, y));
        }
        else if (listNum == 4)
        {
            TseaEdges4.Add(new Vector2(x, y));
        }
    }
    //mainLand SHORE STUFFS
    private static bool HasLand(Texture2D lMap, float x, float y, Color32 sea)
    {
        if(lMap.GetPixel((int)x,(int)y) != sea)
        {
            return true;
        } else
        {
            return false;
        }
    }
    //mainLand SHORE STUFFS
    private static bool NotPainted(float x, float y, Texture2D hMap)
    {
        if(hMap.GetPixel((int)x,(int)y).r == 0) //if the pixel is black
        {
            return true;
        } else
        {
            return false;
        }
    }


    //ICE OBSTACLES
    /*private static void GenerateIceObs(Texture2D hMap)
    {
        Color32 white = new Color32(255, 255, 255, 255);
        //List<Vector2> icePoints = new List<Vector2>();

        float iceLandHeight = 0.125f; //Height
        Color32 col = new Color32((byte)(iceLandHeight * 255), (byte)(iceLandHeight * 255), (byte)(iceLandHeight * 255), 255);

        for(int i = 0; i < icePoints.Count; i++)
        {
            hMap.SetPixel((int)icePoints[i].x, (int)icePoints[i].y, col);
        }
    }*/


    //MOUNTAIN GENERATION
    private static void GenerateMountains(int mountains, int maxMountainLength, int mountainCliffRatio, int mountainTopWidth, float mountainTopFallOffValue, float mountainFallOffValue, int scale, Texture2D hMap, int waviness, Texture2D lMap)
    {
        //System.Random r = new System.Random();
        Vector2[] mountainLocations = new Vector2[mountains]; //Store first/hypo mountain locations 
        Color32 white = new Color32(254, 254, 254, 255);
        Color32 gray = new Color32(125, 125, 125, 255);
        //SaveImage(hMap, "test.png");
        //SaveImage(landMap, Environment.CurrentDirectory + "\\stuff.png");
        terrainGeneration.mountainTopWidth = mountainTopWidth; //Set static values
        terrainGeneration.mountainFallOffValue = mountainFallOffValue;
        terrainGeneration.mountainTopFallOffValue = mountainTopFallOffValue;

        //int finalScale = scale * 512;

        int colInt = r.Next(40, 80); //Height of hills
        bool increasing = true;

        Color32 temp = new Color32((byte)colInt, (byte)colInt, (byte)colInt, 255);
        Vector2 selectedPixel;

        for (int i = 0; i < (int)(mountains * (mountainCliffRatio / 100f)); i++) //Generate cliff tops
        {
            selectedPixel = mainLand[r.Next(0, mainLand.Count - 1)];

            hMap.SetPixel((int)selectedPixel.x, (int)selectedPixel.y, temp);
            
            mountainLocations[i] = selectedPixel;
        }
        finalMountainLocations = new List<Vector2>(mountainLocations);

        int mLeft = mountains -= (int)(mountains * (mountainCliffRatio / 100f));
        Vector2 bl;
        Vector2 tr;
        int mAmount;

        Vector2 rPoint;
        int colorValue;

        for (int i = 0; i < mLeft; i++) //Make mountain ranges
        {
            bl = new Vector2(r.Next(300, finalScale - 300), r.Next(300, finalScale - 300)); //Get the borders
            tr = new Vector2(bl.x + r.Next(50, 300), bl.y + r.Next(50, 300));
            
            mAmount = r.Next(1, 8); //Get the amount of mountains (maxMountainsPerRange?)
            if(mAmount >= mLeft)
            {
                mAmount = mLeft;
            }
            mLeft -= mAmount;
            if(mLeft < 0)
            {
                mLeft = 0;
            }

            for(int p = 0; p < mAmount; p++)
            {
                rPoint = new Vector2(r.Next((int)bl.x, (int)tr.x), r.Next((int)bl.y, (int)tr.y));
                colorValue = r.Next(60, 140); //Height of mountains
                hMap.SetPixel((int)rPoint.x, (int)rPoint.y, new Color32((byte)colorValue, (byte)colorValue, (byte)colorValue, 255));

                finalMountainLocations.Add(new Vector2(rPoint.x, rPoint.y));
            }
        }

        //int test = 0;

        int xCoord = 0;
        int yCoord = 0;

        int revs;

        int dir1;

        int pRevs;
        int pptt;

        //Make cliffs (and mountains apparently)
        Color32 col = new Color32(); 
        for (int v = 0; v < mountainLocations.Length; v++) 
        {
            xCoord = (int)mountainLocations[v].x;
            yCoord = (int)mountainLocations[v].y;

            revs = r.Next(Mathf.RoundToInt(maxMountainLength * 0.75f), maxMountainLength);

            movementPoint = new Vector2(xCoord, yCoord);

            dir1 = r.Next(0, 8);
            pRevs = 0;
            pptt = 0;

            if (waviness > 0) //Waviness of the mountains
            {
                pptt = revs / (waviness + 1);
                pRevs = pptt;
            }

            

            for (int i = 0; i < revs; i++) //Make mountain points
            {
                //CheckCoordinateBools(finalScale); //Check overflow
                    
                if(i + 1 == pRevs) //Waves
                {
                    dir1 = GetDirection(dir1);
                    pRevs += pptt;
                }

                //x1plus = true;
                //x1minus = true;
                //y1minus = true;
                //y1plus = true;
                
                MovePoints((int)movementPoint.x, (int)movementPoint.y,dir1, 0, finalScale, r.Next(0, 5));
                movementPoint = CheckOverflow(movementPoint.x, movementPoint.y);
                //MovePoints(dir2, 1, finalScale, r.Next(0, 5));
                /*temp = lMap.GetPixel((int)movementPoint.x, (int)movementPoint.y);
                if (temp.Equals(sea)) //If mountain is on the sea, lower the height of it
                {
                    if(colInt > 10)
                    {
                        //test++;
                        colInt--;
                    }
                } else
                {*/
                    switch (r.Next(0, 20)) //Randomise mounain top direction (up/down)
                    {
                        case 0:
                            if (increasing)
                            {
                                increasing = false;
                            }
                            else
                            {
                                increasing = true;
                            }
                            break;
                    }

                    if (increasing) //Controls mountain top height
                    {
                        if (colInt < 120)
                        {
                            colInt++;
                        }
                        else
                        {
                            increasing = false;
                        }
                    }
                    else
                    {
                        if (colInt > 80)
                        {
                            colInt--;
                        }
                        else
                        {
                            increasing = true;
                        }
                    }
                //}
                    
                col = new Color32((byte)colInt, (byte)colInt, (byte)colInt, 255);

                hMap.SetPixel((int)movementPoint.x, (int)movementPoint.y, col); //Make the mountains fatter
                lMap.SetPixel((int)movementPoint.x, (int)movementPoint.y, white);
                finalMountainLocations.Add(new Vector2((int)movementPoint.x, (int)movementPoint.y));

                xCoord = (int)movementPoint.x;
                yCoord = (int)movementPoint.y;
            }
            
        }
        //UnityEngine.Debug.Log(test);
        //Make mountain point dimmer randomly outway
        /*bool plusX;
        bool plusY;
        bool minusX;
        bool minusY;*/

        float curPixelBrightness = 1f;
        float minValue = 1f;

        float bf = 0; //brightness fall
        float nb = 0; //new brightness temp

        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        //!!!SET DIMMER PIXELS!!!
        Vector2 selPixel;

        //Stopwatch sw = new Stopwatch();
        //sw.Start();
        //SaveImage(hMap, "\\test.png");
        while(true)
        {
            finalMountainLocations2.Clear();

            for (int v = 0; v < finalMountainLocations.Count; v++)  //Foreach point of a mountain
            {
                xCoord = (int)finalMountainLocations[v].x;
                yCoord = (int)finalMountainLocations[v].y;
                

                curPixelBrightness = hMap.GetPixel(xCoord, yCoord).grayscale;
                minValue = curPixelBrightness * 0.9f; //Define what is a colored pixel

                if(curPixelBrightness < 0.001f)
                {
                    break;
                }

                /*selPixel = CheckOverflow(xCoord + 1, yCoord + 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale <= minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }

                selPixel = CheckOverflow(xCoord + 1, yCoord - 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale <= minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }*/

                selPixel = CheckOverflow(xCoord + 1, yCoord);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale < minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }

                /*selPixel = CheckOverflow(xCoord - 1, yCoord + 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale <= minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }

                selPixel = CheckOverflow(xCoord - 1, yCoord - 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale <= minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }*/

                selPixel = CheckOverflow(xCoord - 1, yCoord);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale < minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }

                selPixel = CheckOverflow(xCoord, yCoord + 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale < minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }

                selPixel = CheckOverflow(xCoord, yCoord - 1);
                if (hMap.GetPixel((int)selPixel.x, (int)selPixel.y).grayscale < minValue)
                {
                    DimmerPixel((int)selPixel.x, (int)selPixel.y, curPixelBrightness, hMap, lMap, gray, white, bf, nb);
                }
            }
            //mountainFallOffValue -= i / 500f;

            terrainGeneration.mountainTopWidth--;
            finalMountainLocations.Clear();
            finalMountainLocations = new List<Vector2>(finalMountainLocations2);
            //UnityEngine.Debug.Log(finalMountainLocations.Count);
            if (finalMountainLocations.Count < 10)
            {
                break;
            }
        }
        //sw.Stop();
        finalMountainLocations.Clear();
        finalMountainLocations2.Clear();
        //UnityEngine.Debug.Log(sw.Elapsed.Seconds);
    }
    //MOUNTAIN GENERATION DIRECTION FUNCTION
    private static int GetDirection(int curDir)
    {
        int dir = 0;
        int ran = r.Next(0, 2);
        if(curDir == 0)
        {
            if(ran == 0)
            {
                dir = 6;
            } else
            {
                dir = 5;
            }
        } else if(curDir == 1)
        {
            if (ran == 0)
            {
                dir = 7;
            }
            else
            {
                dir = 4;
            }
        }
        else if (curDir == 2)
        {
            if (ran == 0)
            {
                dir = 4;
            }
            else
            {
                dir = 5;
            }
        }
        else if (curDir == 3)
        {
            if (ran == 0)
            {
                dir = 7;
            }
            else
            {
                dir = 6;
            }
        }
        else if (curDir == 4)
        {
            if (ran == 0)
            {
                dir = 1;
            }
            else
            {
                dir = 2;
            }
        }
        else if (curDir == 5)
        {
            if (ran == 0)
            {
                dir = 2;
            }
            else
            {
                dir = 0;
            }
        }
        else if (curDir == 6)
        {
            if (ran == 0)
            {
                dir = 3;
            }
            else
            {
                dir = 0;
            }
        }
        else if (curDir == 7)
        {
            if (ran == 0)
            {
                dir = 3;
            }
            else
            {
                dir = 1;
            }
        }

        return dir;
    }
    //MOUNTAIN CLIFF GENERATION
    private static int DimmerPixel(int x, int y, float brightness, Texture2D hMap, Texture2D lMap, Color32 gray, Color32 white, float brightnessFall, float newBrightness) //Dimmer the pixel with a random value and store it for later use
    {
        if (terrainGeneration.mountainTopWidth > 0)
        {
            brightnessFall = brightness - mountainTopFallOffValue; //Fatter tops
            newBrightness = brightnessFall;
            lMap.SetPixel(x, y, white);
        } else
        {
            brightnessFall = brightness - mountainFallOffValue;
            newBrightness = UnityEngine.Random.Range(brightnessFall * 0.95f, brightnessFall);
            lMap.SetPixel(x, y, gray);
        }
        
        if(newBrightness < 0.005f)
        {
            //UnityEngine.Debug.Log("ayy");
            return 0;
        } else if(newBrightness >= 1)
        {
            newBrightness = 1f;
        }

        Color32 colour = new Color32((byte)(newBrightness * 255), (byte)(newBrightness * 255), (byte)(newBrightness * 255), 255);
        
        hMap.SetPixel(x, y, colour);
        
        finalMountainLocations2.Add(new Vector2(x, y));
        return 0;
    }
    //MOUNTAIN TOP GENERATION
    private static void MovePoints(int x1, int y1,int dir, int pointNum, int finalScale, int r)
    {
            if (dir == 0)
            {
                switch (r) //DOWN
                {
                    case 0:
                        //if (x1plus && y1minus) //Bottom-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 - 1);
                        //}
                        break;
                    case 1:
                        //if (y1minus) //Straight Down
                        //{
                            movementPoint = new Vector2(x1, y1 - 1);
                        //}

                        break;
                    case 2:
                        //if (x1minus && y1minus) //Bottom-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 - 1);
                        //}
                        break;

                    case 3:
                        //if (x1plus) //Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1);
                        //}
                        break;
                    case 4:
                        //if (x1minus) //Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1);
                        //}
                        break;
                }
            }
            else if (dir == 1)
            {
                switch (r) //UP
                {
                    case 0:
                        //if (x1plus && y1plus) //Top-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 + 1);
                        //}
                        break;
                    case 1:
                        //if (y1plus) //Straight Up
                        //{
                            movementPoint = new Vector2(x1, y1 + 1);
                        //}
                        break;
                    case 2:
                        //if (x1minus && y1plus) //Top-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 + 1);
                        //}
                        break;
                    case 3:
                        //if (x1plus) //Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1);
                        //}
                        break;
                    case 4:
                        //if (x1minus) //Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1);
                        //}
                        break;

                }
            }
            else if (dir == 2) //RIGHT
            {
                switch (r)
                {
                    case 0:
                        //if (x1plus && y1plus) //Top-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 + 1);
                        //}
                        break;
                    case 1:
                        //if (x1plus && y1minus) //Bottom-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 - 1);
                        //}
                        break;
                    case 2:
                        //if (x1plus) //Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1);
                        //}
                        break;
                    case 3:
                        //if (y1minus) //Straight Down
                        //{
                            movementPoint = new Vector2(x1, y1 - 1);
                        //}
                        break;
                    case 4:
                        //if (y1plus) //Straight Up
                        //{
                            movementPoint = new Vector2(x1, y1 + 1);
                        //}
                        break;
                }
            }
            else if (dir == 3) //LEFT
            {
                switch (r)
                {
                    case 0:
                        //if (x1minus && y1plus) //Top-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 + 1);
                        //}
                        break;
                    case 1:
                        //if (x1minus) //Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1);
                        //}
                        break;
                    case 2:
                        //if (x1minus && y1minus) //Bottom-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 - 1);
                        //}
                        break;
                    case 3:
                        //if (y1minus) //Straight Down
                        //{
                            movementPoint = new Vector2(x1, y1 - 1);
                        //}
                        break;
                    case 4:
                        //if (y1plus) //Straight Up
                        //{
                            movementPoint = new Vector2(x1, y1 + 1);
                        //}
                        break;
                }
            } else if(dir == 4) //TOP-RIGHT
            {
                switch (r)
                {
                    case 0:
                        //if (y1plus) //Straight Up
                        //{
                            movementPoint = new Vector2(x1, y1 + 1);
                        //}
                        break;
                    case 1:
                        //if (x1minus && y1plus) //Top-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 + 1);
                        //}
                        break;
                    case 2:
                        //if (x1plus) //Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1);
                        //}
                        break;
                    case 3:
                        //if (x1plus && y1plus) //Top-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 + 1);
                        //}
                        break;
                    case 4:
                        //if (x1plus && y1minus) //Bottom-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 - 1);
                        //}
                        break;
                }
            } else if(dir == 5) //BOTTOM-RIGHT
            {
                switch (r)
                {
                    case 0:
                        //if (x1plus && y1minus) //Bottom-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 - 1);
                        //}
                        break;
                    case 1:
                        //if (x1plus && y1plus) //Top-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 + 1);
                        //}
                        break;
                    case 2:
                        //if (x1plus) //Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1);
                        //}
                        break;
                    case 3:
                        //if (y1minus) //Straight Down
                        //{
                            movementPoint = new Vector2(x1, y1 - 1);
                        //}
                        break;
                    case 4:
                        //if (x1minus && y1minus) //Bottom-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 - 1);
                        //}
                        break;
                }
            } else if(dir == 6) //BOTTOM-LEFT
            {
                switch (r)
                {
                    case 0:
                        //if (y1minus) //Straight Down
                        //{
                            movementPoint = new Vector2(x1, y1 - 1);
                        //}
                        break;
                    case 1:
                        //if (x1plus && y1minus) //Bottom-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 - 1);
                        //}
                        break;
                    case 2:
                        //if (x1minus && y1minus) //Bottom-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 - 1);
                        //}
                        break;
                    case 3:
                        //if (x1minus) //Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1);
                        //}
                        break;
                    case 4:
                        //if (x1minus && y1plus) //Top-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 + 1);
                        //}
                        break;
                }
            } else if(dir == 7) //TOP-LEFT
            {
                switch (r)
                {
                    case 0:
                        //if (x1minus && y1plus) //Top-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 + 1);
                        //}
                        break;
                    case 1:
                        //if (x1minus) //Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1);
                        //}
                        break;
                    case 2:
                        //if (x1minus && y1minus) //Bottom-Left
                        //{
                            movementPoint = new Vector2(x1 - 1, y1 - 1);
                        //}
                        break;
                    case 3:
                        //if (y1plus) //Straight Up
                        //{
                            movementPoint = new Vector2(x1, y1 + 1);
                        //}
                        break;
                    case 4:
                        //if (x1plus && y1plus) //Top-Right
                        //{
                            movementPoint = new Vector2(x1 + 1, y1 + 1);
                        //}
                        break;
                }
            }
    }


    //mainLand LANDFILL FUNCTION
    //make these work with lakes
    private static void ConnectPoints(List<Vector2> points, Texture2D image, Color32 c)
    {
        for(int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(image, (int)points[i].x, (int)points[i].y, (int)points[i + 1].x, (int)points[i + 1].y, c);
        }
        DrawLine(image, (int)points[0].x, (int)points[0].y, (int)points[points.Count - 1].x, (int)points[points.Count - 1].y, c); 
    }
    static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color32 col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        finalPoints.Add(new Vector2(x0, y0));
        /*if (ice)
        //{
            //if (iceObs)
            //{
                //icePoints.Add(new Vector2(x0, y0));
            //} else
            //{
                //snowLand.Add(new Vector2(x0, y0));
            //}
        }*/

        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);

                finalPoints.Add(new Vector2(x0, y0));
                /*if (ice)
                {
                    if (iceObs)
                    {
                        icePoints.Add(new Vector2(x0, y0));
                    }
                    else
                    {
                        snowLand.Add(new Vector2(x0, y0));
                    //}
                }*/
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);

                finalPoints.Add(new Vector2(x0, y0));
                /*if (ice)
                {
                    if (iceObs)
                    {
                        icePoints.Add(new Vector2(x0, y0));
                    }
                    else
                    {
                        snowLand.Add(new Vector2(x0, y0));
                    //}
                }*/
            }
        }
    }


    //mainLand LANDFILL FUNCTION
    private static void FillArea(List<Vector2> points, Texture2D texture, Color32 color)
    {
        Texture2D image = new Texture2D(finalScale, finalScale);
        Color32 temp = new Color32();

        int count = points.Count;

        for (int r = 0; r < count; r++)
        {
            image.SetPixel((int)points[r].x, (int)points[r].y, color);
            /*if (ice)
            {
                //if (iceObs)
                //{
                    //icePoints.Add(new Vector2(points[r].x, points[r].y));
                //} else
                //{
                    snowLand.Add(new Vector2(points[r].x, points[r].y));
                //}
            }*/
        }
        ConnectPoints(points, image, color);

        //image.Apply();
        //SaveImage(image, Environment.CurrentDirectory + "/test" + color.ToString() + ".png");
        float xMax = 0;
        float yMax = 0;

        float xMin = finalScale;
        float yMin = finalScale;

        float numX;
        float numY;

        float finalX;
        float finalY;

        for (int i = 0; i < points.Count; i++) //Calculate the center of the island
        {
            numX = points[i].x;
            numY = points[i].y;

            if(numX > xMax)
            {
                xMax = numX;
            }
            if (numX < xMin)
            {
                xMin = numX;
            }

            if(numY > yMax)
            {
                yMax = numY;
            }
            if (numY < yMin)
            {
                yMin = numY;
            }
        }

        finalX = (xMin + xMax) / 2;
        finalY = (yMin + yMax) / 2;

        drawPoints1.Add(new Vector2(finalX, finalY)); //Start point
        /*if (ice)
        {
            //if (iceObs)
            //{
                //icePoints.Add(new Vector2(finalX, finalY));
            //}
            //else
            //{
                snowLand.Add(new Vector2(finalX, finalY));
            //}
        }*/
        
        Vector2 startPoint;

        finalPoints.Add(drawPoints1[0]);
        image.SetPixel((int)drawPoints1[0].x, (int)drawPoints1[0].y, color);

        //Debug.Log(drawPoints1[0]); //Start point is the same for each island?
        //Color32 ye = new Color32(255,255,0,255);
        //Color32 gr = new Color32(0, 255, 0, 255);

        /*bool xPlus;
        bool xMinus;
        bool yPlus;
        bool yMinus;*/
        Vector2 pixel;
        //int breaker = 0;
        while(true) //Fix the thing that crashes this thing thanks
        {
            for (int i = 0; i < drawPoints1.Count; i++)
            {
                startPoint = drawPoints1[i];

                /*xPlus = false;
                xMinus = false;
                yPlus = false;
                yMinus = false;

                if (startPoint.x + 1 < finalScale)
                {
                    xPlus = true;
                }
                if (startPoint.x - 1 > 0)
                {
                    xMinus = true;
                }
                if (startPoint.y + 1 < finalScale)
                {
                    yPlus = true;
                }
                if (startPoint.y - 1 > 0)
                {
                    yMinus = true;
                }*/

                //if (xPlus)
                //{
                
                pixel = CheckOverflow(startPoint.x + 1, startPoint.y);
                temp = image.GetPixel((int)pixel.x, (int)pixel.y);
                if (!temp.Equals(color)) //Maybe optimize with .Equals()? -btw doesn't work with !(color).Equals(color) bcuz idk
                {                                                        //Also not even .Equals() works wtf?
                    FillIsland(image, pixel.x, pixel.y, color);
                }
                //}
                //if (xMinus)
                //{ll
                pixel = CheckOverflow(startPoint.x - 1, startPoint.y);
                temp = image.GetPixel((int)pixel.x, (int)pixel.y);
                if (!temp.Equals(color))
                { 
                    FillIsland(image, pixel.x, pixel.y, color);
                }
                //}
                //if (yPlus)
                //{
                pixel = CheckOverflow(startPoint.x, startPoint.y + 1);
                temp = image.GetPixel((int)pixel.x, (int)pixel.y);
                if (!temp.Equals(color))
                {
                    FillIsland(image, pixel.x, pixel.y, color);
                }
                //}
                //if (yMinus)
                //{
                pixel = CheckOverflow(startPoint.x, startPoint.y - 1);
                temp = image.GetPixel((int)pixel.x, (int)pixel.y);
                if (!temp.Equals(color))
                {
                    FillIsland(image, pixel.x, pixel.y, color);
                }
                //}
            }
            //breaker++;
            //UnityEngine.Debug.Log(drawPoints1.Count);
            if(drawPoints2.Count < 1) //Exit the loop
            {
                break;
            }

            //Update lists
            drawPoints1.Clear();
            drawPoints1 = new List<Vector2>(drawPoints2);
            drawPoints2.Clear();
        }
        drawPoints1.Clear();
        drawPoints2.Clear();

        for (int r = 0; r < finalPoints.Count; r++)
        {
            texture.SetPixel((int)finalPoints[r].x, (int)finalPoints[r].y, color);
        }
        
        
    }
    private static void FillIsland(Texture2D image, float x, float y, Color32 col)
    {
        Vector2 vec = new Vector2(x, y);
        finalPoints.Add(vec);
        drawPoints2.Add(vec);
        image.SetPixel((int)x, (int)y, col);

        /*if (ice)
        {
            //if (iceObs)
            //{
                //icePoints.Add(vec);
            //} else
            //{
                snowLand.Add(vec);
            //}
        }*/
    }
    private static Vector2 CheckOverflow(float x, float y)
    {
        if(x < 0)
        {
            x = finalScale - 1;
        } else if(x >= finalScale)
        {
            x = 0;
        }

        if (y < 0)
        {
            y = finalScale - 1;
        }
        else if (y >= finalScale)
        {
            y = 0;
        }

        return new Vector2(x,y);
    }


    //mainLand ISLAND (AND LAKE) EDGE GENERATION
    private static Vector2 MoveEdgePoint(Vector2 drawingPoint, int dir, float unitSize)
    {
        int unit = Mathf.RoundToInt(unitSize);

        if(dir == 0) //UP
        {
            switch (r.Next(0,3))
            {
                case 0: //more up
                    return new Vector2(drawingPoint.x - r.Next(-unit,unit), drawingPoint.y + 2 * unit);
                case 1: //more right
                    return new Vector2(drawingPoint.x - r.Next(-2*unit, unit), drawingPoint.y + 2 * unit);
                case 2: //more left
                    return new Vector2(drawingPoint.x - r.Next(-unit, 2 * unit), drawingPoint.y + 2 * unit);
            }
        }
        else if (dir == 1) //UP-RIGHT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more up
                    return new Vector2(drawingPoint.x + unit, drawingPoint.y + unit + r.Next(-unit, 2*unit));
                case 1: //more right
                    return new Vector2(drawingPoint.x + unit + r.Next(-unit, 2*unit), drawingPoint.y + unit);
                case 2: //more up-right
                    return new Vector2(drawingPoint.x + unit + r.Next(-unit, unit), drawingPoint.y + unit + r.Next(-unit, unit));
            }
        }
        else if(dir == 2) //RIGHT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more up
                    return new Vector2(drawingPoint.x + 2 * unit, drawingPoint.y + r.Next(-unit, 2*unit));
                case 1: //more right
                    return new Vector2(drawingPoint.x + 2 * unit, drawingPoint.y + r.Next(-unit, unit));
                case 2: //more down
                    return new Vector2(drawingPoint.x + 2 * unit , drawingPoint.y + r.Next(-2 * unit, unit));
            }
        }
        else if (dir == 3) //DOWN-RIGHT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more down
                    return new Vector2(drawingPoint.x + unit, drawingPoint.y - unit - r.Next(-unit, 2*unit)); //FINISH TEHSE DIAGONAL MOVEMENT THINGYS
                case 1: //more down-right
                    return new Vector2(drawingPoint.x + unit + r.Next(-unit, unit), drawingPoint.y - unit - r.Next(-unit, unit));
                case 2: //more right
                    return new Vector2(drawingPoint.x + unit + r.Next(-unit, 2*unit), drawingPoint.y - unit);
            }
        }
        else if (dir == 4) //DOWN
        {
            switch (r.Next(0, 3))
            {
                case 0: //more down
                    return new Vector2(drawingPoint.x - r.Next(-unit, unit), drawingPoint.y - 2 * unit);
                case 1: //more right
                    return new Vector2(drawingPoint.x - r.Next(-2 * unit, unit), drawingPoint.y - 2 * unit);
                case 2: //more left
                    return new Vector2(drawingPoint.x - r.Next(-unit, 2 * unit), drawingPoint.y - 2 * unit);
            }
        }
        else if (dir == 5) //DOWN-LEFT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more down
                    return new Vector2(drawingPoint.x - unit, drawingPoint.y - unit - r.Next(-unit, 2 * unit)); //FINISH TEHSE DIAGONAL MOVEMENT THINGYS
                case 1: //more down-right
                    return new Vector2(drawingPoint.x - unit - r.Next(-unit, unit), drawingPoint.y - unit - r.Next(-unit, unit));
                case 2: //more left
                    return new Vector2(drawingPoint.x - unit - r.Next(-unit, 2 * unit), drawingPoint.y - unit);
            }
        }
        else if (dir == 6) //LEFT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more up
                    return new Vector2(drawingPoint.x - 2 * unit, drawingPoint.y + r.Next(-unit, 2 * unit));
                case 1: //more left
                    return new Vector2(drawingPoint.x - 2 * unit, drawingPoint.y + r.Next(-unit, unit));
                case 2: //more down
                    return new Vector2(drawingPoint.x - 2 * unit, drawingPoint.y + r.Next(-2 * unit, unit));
            }
        }
        else if (dir == 7) //UP-LEFT
        {
            switch (r.Next(0, 3))
            {
                case 0: //more up
                    return new Vector2(drawingPoint.x - unit, drawingPoint.y + unit + r.Next(-unit, 2*unit));
                case 1: //more up-left
                    return new Vector2(drawingPoint.x - unit - r.Next(-unit, unit), drawingPoint.y + unit + r.Next(-unit, unit));
                case 2: //more left
                    return new Vector2(drawingPoint.x - unit - r.Next(-unit, 2*unit), drawingPoint.y + unit);
            }
        }
        UnityEngine.Debug.Log("MoveEdgePoint Error: No dir specified");
        return new Vector2(0, 0);
    }
    //mainLand ISLAND (AND LAKE) LAST EDGE
    private static Vector2 FinishIsland(Vector2 curPoint, Vector2 finalPoint, int unit, int idetail)
    {
        Vector2 returnPoint = curPoint;
        int closenessValue = idetail;
        int connectionValue = closenessValue / 5;

        int xClose = 0; //0 not close (move at regular speed), 1 pretty close (move at pixel speed), 2 really close (connect x-x, y-y)
        int yClose = 0;

        int xMore = 0; //1,2 are directions, 3 is to not move
        int yMore = 0;

        if(finalPoint.x >= curPoint.x)
        {
            xMore = 1;
            if(finalPoint.x - curPoint.x < closenessValue) //<-Closeness value (change to pixelPerRevMode)
            {
                xClose = 1;
                if(finalPoint.x - curPoint.x < connectionValue)
                {
                    xClose = 2;
                }
            } 
        }
        if(finalPoint.x <= curPoint.x)
        {
            xMore = 0;
            if(curPoint.x - finalPoint.x < closenessValue)
            {
                xClose = 1;
                if(curPoint.x - finalPoint.x < connectionValue)
                {
                    xClose = 2;
                }
            }
        }
        if(finalPoint.y >= curPoint.y)
        {
            yMore = 1;
            if(finalPoint.y - curPoint.y < closenessValue)
            {
                yClose = 1;
                if(finalPoint.y - curPoint.y < connectionValue)
                {
                    xClose = 2;
                }
            } 
        }
        if(finalPoint.y <= curPoint.y)
        {
            yMore = 0;
            if(curPoint.y - finalPoint.y < closenessValue)
            {
                yClose = 1;
                if(curPoint.y - finalPoint.y < connectionValue)
                {
                    xClose = 2;
                }
            }
        }

        if(xClose == 1 || yClose == 1)
        {
            if (xClose == 1)
            {
                xMore = 3;
                if(xClose == 2)
                {
                    curPoint.x = finalPoint.x;
                }
            }
            if (yClose == 1)
            {
                yMore = 3;
                if(yClose == 2)
                {
                    curPoint.y = finalPoint.y;
                }
            }
            if(xClose == 1 && yClose == 1)
            {
                unit = 1;
            }
        }

        if (xMore == 1)
        {
            returnPoint = MoveEdgePoint(returnPoint, 2, unit * 0.5f);
        }
        else if (xMore == 0)
        {
            returnPoint = MoveEdgePoint(returnPoint, 6, unit * 0.5f);
        }
        if (yMore == 1)
        {
            returnPoint = MoveEdgePoint(returnPoint, 0, unit * 0.5f);
        }
        else if (yMore == 0)
        {
            returnPoint = MoveEdgePoint(returnPoint, 4, unit * 0.5f);
        }

        return returnPoint;
    }
    //mainLand ISLAND (AND LAKE) CLOSENESS CHECKER
    private static bool IsClose(Vector2 curPoint, Vector2 startPoint, int closeOffVal)
    {
        bool xClose = false;
        bool yClose = false;
        
        if(curPoint.x >= startPoint.x)
        {
            if (curPoint.x - startPoint.x < closeOffVal)
            {
                xClose = true;
            }
        } else if(startPoint.x >= curPoint.x)
        {
            if(startPoint.x - curPoint.x < closeOffVal)
            {
                xClose = true;
            }
        }
        if (curPoint.y >= startPoint.y)
        {
            if (curPoint.y - startPoint.y < closeOffVal)
            {
                yClose = true;
            }
        }
        else if (startPoint.y >= curPoint.y)
        {
            if (startPoint.y - curPoint.y < closeOffVal)
            {
                yClose = true;
            }
        }

        if(xClose && yClose)
        {
            return true;
        }

        return false;
    }
    //mainLand ISLAND EDGE VARIETY
    private static int CheckEdgeGenType(int curDir, string type, int islandShapeChanging, int edgesDone)
    {
        if (type == "random")
        {
            if (islandShapeChanging > 0) //Crazy island edges
            {
                int randomTurn = r.Next(0, islandShapeChanging + 1); //Chances for a random turn
                if (randomTurn == 0)
                {
                    //if (dir > 4)
                    //{
                    //    idetail += idetail / 5;
                    //}
                    switch (r.Next(0, 2))
                    {
                        case 0:
                            if (curDir < 7)
                            {
                                curDir++;
                            }
                            else if (curDir != 0)
                            {
                                curDir--;
                            }
                            break;
                        case 1:
                            if (curDir != 0)
                            {
                                curDir--;
                            }
                            else
                            {
                                curDir++;
                            }
                            break;
                    }
                }
            }
        } else if(type == "edgeSplit")
        {
            if(curDir <= 6)
            {
                if(splitting)
                {
                    if(edgeDirChanged < splitsPerEdgeS)
                    {
                        int switchNum = 0;
                        if (islandShapeChanging > 0)
                        {
                            if (r.Next(0, islandShapeChanging) == 0)
                            {
                                switchNum = r.Next(0, 2);
                            }
                            else
                            {
                                switchNum = 2;
                            }
                        }
                        else
                        {
                            switchNum = r.Next(0, 2);
                        }
                        switch (switchNum)
                        {
                            /*Entire Project Made By: Roni Tuohino
                            Hash signature:
                            a6ad6e5bf4e07f9b1b28f0f14916c8960097edd973b1431927273e7b62c8d701*/
                            case 0:
                                curDir++;
                                edgeDirInt++;
                                break;
                            case 1:
                                curDir--;
                                edgeDirInt--;
                                break;
                            case 2:
                                break;
                        }
                        edgeDirChanged++;
                        lastTimeDirChanged = edgesDone;
                    } else
                    {
                        splitting = false;
                    }
                } else
                {
                    if(edgesDone != lastTimeDirChanged)
                    {
                        if (edgeDirInt < 0)
                        {
                            curDir++;
                            edgeDirInt++;
                        }
                        else if(edgeDirInt > 0)
                        {
                            curDir--;
                            edgeDirInt--;
                        } else
                        {
                            splitting = true;
                        }
                    }
                }
            } 
        }

        return curDir;
    }
    //mainLand LAKE SPAWN CHECKER
    private static bool CheckLakeSpawn(Vector2 lakeStart, int dist, int finalScale, Texture2D bMap)
    {
        //Debug.Log("Checking");
        //Checks the lakeSpawn and if it is close to water(sea(maybe lakes?))
        Vector2 coord = new Vector2(lakeStart.x, finalScale - lakeStart.y);

        Color32 blue = new Color32(0, 0, 255, 255);
        Color32 temp = new Color32();

        bool checkXplus = true;
        bool checkXminus = true;

        bool checkYplus = true;
        bool checkYminus = true;

        int border = dist;

        if (coord.x < border)
        {
            checkXminus = false;
        }
        if(finalScale - coord.x < border)
        {
            checkXplus = false;
        }
        if(coord.y < border)
        {
            checkYminus = false;
        }
        if(finalScale - coord.y < border)
        {
            checkYplus = false;
        }

        for(int x = 0; x < dist; x++)
        {
            for(int y = 1; y < dist; y++)
            {
                if(checkXplus && checkYminus)
                {
                    temp = bMap.GetPixel((int)coord.x + x, (int)coord.y - y);
                    if (temp.Equals(blue)) //Check down-right
                    {
                        return true;
                    }
                }
                if (checkXplus)
                {
                    temp = bMap.GetPixel((int)coord.x + x, (int)coord.y);
                    if (temp.Equals(blue)) //Check right
                    {
                        return true;
                    }
                }
                if (checkYminus)
                {
                    temp = bMap.GetPixel((int)coord.x, (int)coord.y - y);
                    if (temp.Equals(blue)) //Check down
                    {
                        return true;
                    }
                }
                if(checkXminus && checkYminus)
                {
                    temp = bMap.GetPixel((int)coord.x - x, (int)coord.y - y);
                    if (temp.Equals(blue)) //Check down-left
                    {
                        return true;
                    }
                }
                if (checkYplus)
                {
                    temp = bMap.GetPixel((int)coord.x, (int)coord.y + y);
                    if (temp.Equals(blue)) //Check up
                    {
                        return true;
                    }
                }
                if(checkXplus && checkYplus)
                {
                    temp = bMap.GetPixel((int)coord.x + x, (int)coord.y + y);
                    if (temp.Equals(blue)) //Check up-right
                    {
                        return true;
                    }
                }
                if(checkXminus && checkYplus)
                {
                    temp = bMap.GetPixel((int)coord.x - x, (int)coord.y + y);
                    if (temp.Equals(blue)) //Check up-left
                    {
                        return true;
                    }
                }
                if (checkXminus)
                {
                    temp = bMap.GetPixel((int)coord.x - x, (int)coord.y);
                    if (temp.Equals(blue)) //Check left
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    //BIOMEMAP FADE MAINFRAME
    public static Texture2D BiomeFade(Texture2D lMap) //A few ways of doing this:
        //1. Store biome edges and expand from those with corresponding colors
        //2. Store biome edges and expand only to specific biomes making the other more dominant
            //We could balance the generation to make them 50:50 again
            //This is de wey

    {
        Color32 green = new Color32(0, 255, 0, 255);
        Color32 yellow = new Color32(255, 255, 0, 255);
        Color32 white = new Color32(255, 255, 255, 255);

        Color32 tempColor = new Color32();

        int switcher = 0;

        float gyVal = 205;
        float ywVal = 205;
        float gwVal = 205;

        float decrease = 50;

        bool skip;
        /*bool xPlus;
        bool xMinus;
        bool yPlus;
        bool yMinus;*/

        Color32 gyC = new Color32((byte)gyVal, 255, 0, 255);
        Color32 ywC = new Color32(255, 255, (byte)ywVal, 255);
        Color32 gwC = new Color32((byte)gwVal, 255, (byte)gwVal, 255);

        Color32 pixelColor = new Color32();

        Vector2 temp = new Vector2(0,0);

        //Store biome edges
        for (int x = 0; x < finalScale; x++)
        {
            for (int y = 0; y < finalScale; y++)
            {
                tempColor = lMap.GetPixel(x, y);

                //Check for yellow-green edges
                if (tempColor.Equals(yellow))
                {
                    skip = false;

                    temp = CheckOverflow(x + 1, y);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                    if (pixelColor.Equals(green))
                    {
                        FDEdges.Add(temp);
                        skip = true;
                    }

                    if (!skip)
                    {
                    temp = CheckOverflow(x - 1, y);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(green))
                        {
                            FDEdges.Add(temp);
                            skip = true;
                        }
                    }
                    if (!skip)
                    {
                    temp = CheckOverflow(x, y + 1);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(green))
                        {
                            FDEdges.Add(temp);
                            //skip = true;
                        }
                    }
                    if (!skip)
                    {
                    temp = CheckOverflow(x, y - 1);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(green))
                        {
                            FDEdges.Add(temp);
                            //skip = true;
                        }
                    }
                }

                //Check for green-white edges
                if (tempColor.Equals(green))
                {
                    skip = false;

                    temp = CheckOverflow(x + 1, y);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                    if (pixelColor.Equals(white))
                    {
                        FSEdges.Add(temp);
                        skip = true;
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x - 1, y);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(white))
                        {
                            FSEdges.Add(temp);
                            skip = true;
                        }
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x, y + 1);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(white))
                        {
                            FSEdges.Add(temp);
                            //skip = true;
                        }
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x, y - 1);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(white))
                        {
                            FSEdges.Add(temp);
                            //skip = true;
                        }
                    }
                }

                //Check for white-yellow edges
                if (tempColor.Equals(white))
                {
                    skip = false;

                    temp = CheckOverflow(x + 1, y);
                    pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                    if (pixelColor.Equals(yellow))
                    {
                        DSEdges.Add(temp);
                        skip = true;
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x - 1, y);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(yellow))
                        {
                            DSEdges.Add(temp);
                            skip = true;
                        }
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x, y + 1);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(yellow))
                        {
                            DSEdges.Add(temp);
                            //skip = true;
                        }
                    }
                    if (!skip)
                    {
                        temp = CheckOverflow(x, y - 1);
                        pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                        if (pixelColor.Equals(yellow))
                        {
                            DSEdges.Add(temp);
                            //skip = true;
                        }
                    }
                }
            }
        }
        

        //Blend the biomes
        for (int rev = 0; rev < 16; rev++)
        {
            if(switcher == 4)
            {
                gyVal -= decrease;
                ywVal -= decrease;
                gwVal -= decrease;

                gyC = new Color32((byte)gyVal, 255, 0, 255);
                ywC = new Color32(255, 255, (byte)ywVal, 255);
                gwC = new Color32((byte)gwVal, 255, (byte)gwVal, 255);

                switcher -= 4;
            } else
            {
                switcher++;
            }
            

            for (int i = 0; i < FDEdges.Count; i++)
            {
                int pixelX = (int)FDEdges[i].x;
                int pixelY = (int)FDEdges[i].y;

                temp = CheckOverflow(pixelX + 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gyC, 0);
                }
                temp = CheckOverflow(pixelX - 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gyC, 0);
                }
                temp = CheckOverflow(pixelX, pixelY + 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gyC, 0);
                }
                temp = CheckOverflow(pixelX, pixelY - 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gyC, 0);
                }
            }
            for (int i = 0; i < FSEdges.Count; i++)
            {
                int pixelX = (int)FSEdges[i].x;
                int pixelY = (int)FSEdges[i].y;

                temp = CheckOverflow(pixelX + 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gwC, 1);
                }
                temp = CheckOverflow(pixelX - 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gwC, 1);
                }
                temp = CheckOverflow(pixelX, pixelY + 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gwC, 1);
                }
                temp = CheckOverflow(pixelX, pixelY - 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(green))
                {
                    Fade(temp, lMap, gwC, 1);
                }
            }
            for (int i = 0; i < DSEdges.Count; i++)
            {
                int pixelX = (int)DSEdges[i].x;
                int pixelY = (int)DSEdges[i].y;

                temp = CheckOverflow(pixelX + 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(yellow))
                {
                    Fade(temp, lMap, ywC, 2);
                }
                temp = CheckOverflow(pixelX - 1, pixelY);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(yellow))
                {
                    Fade(temp, lMap, ywC, 2);
                }
                temp = CheckOverflow(pixelX, pixelY + 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(yellow))
                {
                    Fade(temp, lMap, ywC, 2);
                }
                temp = CheckOverflow(pixelX, pixelY - 1);
                pixelColor = lMap.GetPixel((int)temp.x, (int)temp.y);
                if (pixelColor.Equals(yellow))
                {
                    Fade(temp, lMap, ywC, 2);
                }
            }

            FDEdges.Clear();
            FDEdges = new List<Vector2>(DumpFDEdges);
            DumpFDEdges.Clear();

            FSEdges.Clear();
            FSEdges = new List<Vector2>(DumpFSEdges);
            DumpFSEdges.Clear();

            DSEdges.Clear();
            DSEdges = new List<Vector2>(DumpDSEdges);
            DumpDSEdges.Clear();
        }

        FDEdges.Clear();
        FSEdges.Clear();
        DSEdges.Clear();

        return lMap;
    }
    //BIOMEMAP FADE FUNCTION
    private static void Fade(Vector2 coords, Texture2D lMap, Color32 color, int relation)
    {
        switch (relation)
        {
            case 0:
                DumpFDEdges.Add(coords);
                break;
            case 1:
                DumpFSEdges.Add(coords);
                break;
            case 2:
                DumpDSEdges.Add(coords);
                break;
        }
        //biomeEdges2.Add(new Vector2(pixX, pixY));
        lMap.SetPixel((int)coords.x, (int)coords.y, color);
    }


    //PUBLIC SAVE IMAGE FUNCTION
    public static void SaveImage(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }


    //PERLIN NOISE
    static Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(finalScale, finalScale);       //make new texture with [width] and [height]

        float randX = r.Next(0, 999);
        float randY = r.Next(0, 999);

        for (int x = 0; x < finalScale; x++)
        {//as long as x is less than 256, increase x and run the code within the for loop

            for (int y = 0; y < finalScale; y++)
            {//as long as y is less than 256, and x is less than 256, increase y and run the code within the for loop

                Color32 color = CalculateColor(x, y, randX, randY); //run the Calculate Color function to decide the color
                texture.SetPixel(x, y, color);      //place a pixel at [x] and [y] with the [color]
            }

        }

        texture.Apply();     //applies the texture
        return texture;      //returns the texture
    }
    static Color32 CalculateColor(int x, int y, float randX, float randY)
    {
        float xCord = randX + (float)x / finalScale * 3;         //set xCord to x/width, and make sure it is a dicimal. [With Scale]
        float yCord = randY + (float)y / finalScale * 3;       //set yCord to y/height, and make sure it's a decimal. [With Scale]

        float sample = Mathf.PerlinNoise(xCord, yCord);        //set sample to a random number between [x] and [y]
        return new Color32((byte)(sample * 255), (byte)(sample * 255), (byte)(sample * 255), 255);             //0 = black, 1 = white

    }
}
