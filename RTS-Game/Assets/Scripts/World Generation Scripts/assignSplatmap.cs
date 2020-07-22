using UnityEngine;
using System.Collections.Generic;
using System.Linq; // used for Sum of array
using System;

public class assignSplatmap : MonoBehaviour
{
    
    public static void AssignMap(GameObject terrainListGameObject ,Texture2D lMap, bool generated, int mapSize)
    {
        List<SplatPrototype> splats = new List<SplatPrototype>(); //Have a number for the corresponding color
        List<Texture2D> splatTextures = new List<Texture2D>();
        List<DataPoint> data = new List<DataPoint>();

        List<int> diff;

        Color32 tempColor = new Color32();
        Color32 curPixel = new Color32();

        Texture2D text;
        SplatPrototype sp;

        int temp;
        
        int rev = 0;
        int revMinus = 0;

        int finalScale = lMap.width;

        foreach (Terrain t in terrainListGameObject.GetComponentsInChildren<Terrain>())
        {
            //Clear lists
            splatTextures.Clear();
            splats.Clear();
            data.Clear();

            //Set settings for terrains
            t.castShadows = true; 
            t.heightmapPixelError = 0.8f; //Gives frames
            t.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse; //Remove shininess

            // Get a reference to the terrain data
            TerrainData terrainData = t.terrainData;

            

            //x,y are positions, third value is the texture to use, the value stored is brightness of the texture
            //We need to resize this

            //Locate the current area in the biomeMap

            int xRev = 0;
            int yRev = 0;

            if (rev > mapSize-1)
            {
                while (rev > mapSize-1)
                {
                    yRev++;
                    rev -= mapSize;

                    revMinus++;
                }
            }
            xRev = rev;

            //reset rev
            while (revMinus > 0)
            {
                rev += mapSize;
                revMinus--;
            }

            xRev *= 512;
            yRev *= 512;
            int tre = 0;
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrainData.alphamapWidth; x++)
                {
                    tre++;
                    //Set colors according to biomeMap
                    curPixel = lMap.GetPixel(y + yRev, x + xRev); //Fixes the orientation WOOOO

                    temp = FindTexture(curPixel, tempColor ,splatTextures);
                    if (temp >= 0)
                    {
                        data.Add(new DataPoint(x, y, temp));
                        //splatmapData[x, y, temp] = 1f;
                    } else
                    {
                        text = new Texture2D(1, 1);
                        text.SetPixel(0, 0, curPixel);
                        text.Apply();

                        sp = new SplatPrototype();
                        sp.texture = text;

                        splats.Add(sp);
                        splatTextures.Add(text);

                        //temp = FindTexture(curPixel, tempColor, splatTextures);
                        data.Add(new DataPoint(x, y, splatTextures.Count - 1));
                    }
                }
            }
            //UnityEngine.Debug.Log("Terrain: " + rev + ", dataSize: " + data.Count + ", tre: " + tre);
            diff = new List<int>();
            for(int i = 0; i < data.Count; i++)
            {
                if (!diff.Contains(data[i].splat))
                {
                    diff.Add(data[i].splat);
                }
            }

            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, diff.Count];
            for(int i = 0; i < data.Count; i++)
            {
                splatmapData[data[i].x, data[i].y, data[i].splat] = 1f;
            }

            //Finally assign the new splatmap to the terrainData:
            terrainData.splatPrototypes = splats.ToArray();
            terrainData.SetAlphamaps(0, 0, splatmapData);
            rev++;
        }
    }

    static Texture2D GenerateTexture(Color32 col)
    {
        Texture2D text = new Texture2D(1, 1);
        text.SetPixel(0, 0, col);
        //text.Apply();

        return text;
    }

    static int FindTexture(Color32 col, Color32 temp, List<Texture2D> splatTextures)
    {
        for(int i = 0; i < splatTextures.Count; i++)
        {
            temp = splatTextures[i].GetPixel(0, 0);
            if (temp.Equals(col))
            {
                return i;
            }
        }
        return -1;
    }
}

public class DataPoint
{
    public int x { get; set; }
    public int y { get; set; }
    public int splat { get; set; }

    public DataPoint(int xCoord, int yCoord, int splatValue)
    {
        x = xCoord;
        y = yCoord;
        splat = splatValue;
    }
}

// CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

// Texture[0] has constant influence
//splatWeights[0] = 1f;

// Texture[1] is stronger at lower altitudes
//splatWeights[1] = 0.5f;//Mathf.Clamp01((terrainData.heightmapHeight - height));

// Texture[2] stronger on flatter terrain
// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
// Subtract result from 1.0 to give greater weighting to flat surfaces
//splatWeights[2] = 0f;//1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight / 5.0f));

// Texture[3] increases with height but only on surfaces facing positive Z axis 
//splatWeights[3] = 0f;//height * Mathf.Clamp01(normal.z);

// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
//float z = splatWeights.Sum();

// Loop through each terrain texture
//for (int i = 0; i < terrainData.alphamapLayers; i++)
//{

// Normalize so that sum of all texture weights = 1
//splatWeights[i] /= z;

// Assign this point to the splatmap array
//splatmapData[x, y, i] = splatWeights[i];
//}
///nibbas ballin`
/// oon hommo
/// ¨xdd pränkki
/// Whaat
/// nou manii
/// YEEt
/// SUCK ACOCK
/// 
///
///
