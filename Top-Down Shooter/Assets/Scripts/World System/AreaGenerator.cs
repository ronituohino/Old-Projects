using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Generates the actual playing areas, in which the player shoots & loots
// Stores all building prefabs
public class AreaGenerator : Singleton<AreaGenerator>
{
    public Vector2Int areaSize;

    Area.Type[,] typeField;

    public List<GameObject> debugTiles = new List<GameObject>();
    public Sprite debugTile;

    [Header("City mass")]

    [SerializeField] float perlinScale;
    [SerializeField] Texture2D densityTexture;
    [SerializeField] float densityMultiplier;
    [SerializeField] float threshold;

    Vector2Int bottomLeftBoundingPoint;
    Vector2Int topRightBoundingPoint;

    [Header("Roads")]

    [SerializeField] int roadSubIterations;
    Vector2Int generationDirection = new Vector2Int(1, 1);

    [Space]

    [SerializeField] int minDistanceForSeeds;
    [SerializeField] float chanceOfSeed;

    [Space]

    [SerializeField] float chanceOfSuddenStop;

    [Space]

    [SerializeField] bool trimRoads;

    [Header("Buildings")]

    [SerializeField] BuildingSet buildingSet;

    [Space]

    [SerializeField] float chanceOfBuildingSeed;
    [SerializeField] Vector2Int maxBuildingSize;

    [Space]

    //Describes how building size and density is effected by the distance from the center of the city
    [SerializeField] AnimationCurve buildingCurve;

    [SerializeField] float buildingSizeMultiplier;

    [Range(0f, 1f)]
    [SerializeField] float buildingSizeRandomEffect;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            WorldManager.Instance.LoadArea(GenerateArea(Mathf.RoundToInt(Random.Range(0, 10000)), true));
        }
    }

    public Area GenerateArea(int seed, bool debug)
    {
        if (!debug)
        {
            return null;
        }

        Area area = new Area();
        Random.InitState(seed);
        area.seed = seed;

        int size = Mathf.RoundToInt(Random.Range(areaSize.x, areaSize.y));
        area.size = new Vector2Int(size, size);

        area.name = seed.ToString();
        area.areaType = Area.AreaType.Unknown;

        typeField = new Area.Type[area.size.x, area.size.y];
        area.buildings = new List<Area.Building>();

        int minX = areaSize.x;
        int maxX = 0;

        int minY = areaSize.y;
        int maxY = 0;

        for (int y = 0; y < area.size.y; y++)
        {
            for (int x = 0; x < area.size.x; x++)
            {
                //City area
                float perlin = Mathf.PerlinNoise(x * perlinScale + (seed), y * perlinScale + (seed));
                float densityVal = densityTexture.GetPixel(Mathf.RoundToInt((float)x / (float)area.size.x * densityTexture.width), Mathf.RoundToInt((float)y / (float)area.size.y * densityTexture.height)).grayscale;

                if (densityMultiplier * densityVal * perlin > threshold)
                {
                    if (x < minX)
                    {
                        minX = x;
                    }

                    if (x > maxX)
                    {
                        maxX = x;
                    }

                    if (y < minY)
                    {
                        minY = y;
                    }

                    if (y > maxY)
                    {
                        maxY = y;
                    }

                    typeField[x, y] = Area.Type.City;
                }
                else
                {
                    typeField[x, y] = Area.Type.Empty;
                }
            }
        }

        //For optimization purposes
        bottomLeftBoundingPoint = new Vector2Int(minX, minY);
        topRightBoundingPoint = new Vector2Int(maxX, maxY);



        //Roads
        List<Vector2Int> seeds = new List<Vector2Int>();

        seeds.Add(bottomLeftBoundingPoint);

        bool[,] roads = new bool[areaSize.x, areaSize.y];
        GenerateRoad(seeds, generationDirection, 0, ref roads);

        if (trimRoads)
        {
            TrimRoads();
        }



        List<Area.Building> b = GenerateBuildings();
        area.buildings.AddRange(b);


        //Set debug slots for areas
        for (int y = 0; y < area.size.y; y++)
        {
            for (int x = 0; x < area.size.x; x++)
            {
                Area.Building building = new Area.Building();

                building.debug = (int)typeField[x, y];
                building.position = new Vector2(x, y);

                area.buildings.Add(building);
            }
        }

        return area;
    }



    bool InBounds(int x, int y)
    {
        if (
            x < bottomLeftBoundingPoint.x ||
            x >= topRightBoundingPoint.x ||
            y < bottomLeftBoundingPoint.y ||
            y >= topRightBoundingPoint.y
           )
        {
            return false;
        }

        return true;
    }

    bool InBounds(Vector2Int position)
    {
        return InBounds(position.x, position.y);
    }



    //Road generation
    void GenerateRoad(List<Vector2Int> seeds, Vector2Int generationDirection, int iteration, ref bool[,] roads)
    {
        List<Vector2Int> subSeeds = new List<Vector2Int>();

        bool even = iteration % 2 == 0;
        Vector2Int direction = (even ? new Vector2Int(generationDirection.x, 0) : new Vector2Int(0, generationDirection.y));

        int seedCount = seeds.Count;
        for (int i = 0; i < seedCount; i++)
        {
            Vector2Int position = seeds[i];

            int seedIterator = (iteration == 0 ? minDistanceForSeeds : 0);
            int len = even ? areaSize.x : areaSize.y;

            for (int l = 0; l < len; l++)
            {
                Area.Type plot = typeField[position.x, position.y];

                if (iteration > 0 && Random.Range(0f, 1f) < chanceOfSuddenStop)
                {
                    break;
                }

                if (plot == Area.Type.City)
                {
                    typeField[position.x, position.y] = Area.Type.Road;
                }
                //If we hit a road, stop generating road
                else if (l > 0 && roads[position.x, position.y])
                {
                    break;
                }

                roads[position.x, position.y] = true;

                if (seedIterator >= minDistanceForSeeds && !UpcomingRoadInRange(position, direction, minDistanceForSeeds, ref roads))
                {
                    if (Random.Range(0f, 1f) < chanceOfSeed)
                    {
                        subSeeds.Add(position);
                        seedIterator = 0;
                    }
                }

                position += direction;

                //Roads are useless outside city-mass bounds
                if (!InBounds(position))
                {
                    break;
                }

                seedIterator++;
            }
        }

        iteration++;
        if (iteration < roadSubIterations)
        {
            GenerateRoad(subSeeds, generationDirection, iteration, ref roads);
        }
    }

    bool UpcomingRoadInRange(Vector2Int position, Vector2Int direction, int distance, ref bool[,] roads)
    {
        for (int i = 1; i < distance; i++)
        {
            Vector2Int positionToCheck = position + direction * i;

            if (!InBounds(positionToCheck))
            {
                return false;
            }

            if (roads[positionToCheck.x, positionToCheck.y])
            {
                return true;
            }
        }

        return false;
    }

    void TrimRoads()
    {
        for (int y = bottomLeftBoundingPoint.y; y <= topRightBoundingPoint.y; y++)
        {
            for (int x = bottomLeftBoundingPoint.x; x <= topRightBoundingPoint.x; x++)
            {
                if (typeField[x, y] == Area.Type.Road)
                {
                    TrimCheck(x, y);
                }
            }
        }
    }

    void TrimCheck(int x, int y)
    {
        int connectionAmount = 0;
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (typeField[x + 1, y] == Area.Type.Road)
        {
            neighbours.Add(new Vector2Int(x + 1, y));
            connectionAmount++;
        }

        if (typeField[x - 1, y] == Area.Type.Road)
        {
            neighbours.Add(new Vector2Int(x - 1, y));
            connectionAmount++;
        }

        if (typeField[x, y + 1] == Area.Type.Road)
        {
            neighbours.Add(new Vector2Int(x, y + 1));
            connectionAmount++;
        }

        if (typeField[x, y - 1] == Area.Type.Road)
        {
            neighbours.Add(new Vector2Int(x, y - 1));
            connectionAmount++;
        }

        if (connectionAmount < 2)
        {
            typeField[x, y] = Area.Type.City;

            for (int i = 0; i < connectionAmount; i++)
            {
                Vector2Int v = neighbours[i];
                TrimCheck(v.x, v.y);
            }
        }
    }



    //Building placement 
    List<Area.Building> GenerateBuildings()
    {
        List<Area.Building> buildings = new List<Area.Building>();

        for (int y = bottomLeftBoundingPoint.y; y < topRightBoundingPoint.y; y++)
        {
            for (int x = bottomLeftBoundingPoint.x; x < topRightBoundingPoint.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if
                   (
                        typeField[x, y] != Area.Type.Road
                        &&
                        typeField[x, y] != Area.Type.Building
                        &&
                        (
                            typeField[x + 1, y] == Area.Type.Road ||
                            typeField[x - 1, y] == Area.Type.Road ||
                            typeField[x, y + 1] == Area.Type.Road ||
                            typeField[x, y - 1] == Area.Type.Road
                        )
                   )
                {
                    //Calculate probabilities for buildings, more and bigger buildings in the middle
                    float densityProbability = Random.Range(0f, 1f);
                    float distanceFromCenter = Vector2.Distance((Vector2)areaSize / 2f, new Vector2(x, y));
                    float distanceClamped = Mathf.Clamp(distanceFromCenter, 0f, areaSize.x / 2f);

                    float distanceFactor = buildingCurve.Evaluate(Extensions.Remap(distanceClamped, 0f, areaSize.x / 2f, 0f, 1f));

                    if (densityProbability * distanceFactor < chanceOfBuildingSeed)
                    {
                        //Probabilites for size of building
                        float sizeProbability = Random.Range(0f, buildingSizeRandomEffect);

                        float step = 1f / maxBuildingSize.y;

                        int maxSize = Mathf.Clamp
                                            (
                                                Mathf.RoundToInt
                                                    (
                                                        Mathf.Clamp01((1 - sizeProbability) * (1 - distanceFactor) * buildingSizeMultiplier) / step
                                                    ),
                                            maxBuildingSize.x, maxBuildingSize.y);

                        buildings.Add(GenerateSingleBuilding(position, maxSize));
                    }
                }
            }
        }

        return buildings;
    }

    //Generates a rectangle for a building, seed in position
    Area.Building GenerateSingleBuilding(Vector2Int position, int maxSize)
    {
        Area.Building building = new Area.Building();

        //Directions of expansion
        bool xPos = true;
        bool xNeg = true;
        bool yPos = true;
        bool yNeg = true;

        Vector2Int bottomLeft = position;
        Vector2Int topRight = position;

        //Expand rect within roads
        while (xPos || xNeg || yPos || yNeg)
        {
            //Top
            if (yPos)
            {
                bool canExpand = true;
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    if (
                            !InBounds(x, topRight.y + 1) ||
                            typeField[x, topRight.y + 1] == Area.Type.Road ||
                            typeField[x, topRight.y + 1] == Area.Type.Building
                       )
                    {
                        canExpand = false;
                        break;
                    }
                }

                if (canExpand)
                {
                    topRight.y++;
                }
                else
                {
                    yPos = false;
                }
            }

            //Right
            if (xPos)
            {
                bool canExpand = true;
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    if (
                            !InBounds(topRight.x + 1, y) ||
                            typeField[topRight.x + 1, y] == Area.Type.Road ||
                            typeField[topRight.x + 1, y] == Area.Type.Building
                       )
                    {
                        canExpand = false;
                        break;
                    }
                }

                if (canExpand)
                {
                    topRight.x++;
                }
                else
                {
                    xPos = false;
                }
            }

            //Bottom
            if (yNeg)
            {
                bool canExpand = true;
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    if (
                            !InBounds(x, bottomLeft.y - 1) ||
                            typeField[x, bottomLeft.y - 1] == Area.Type.Road ||
                            typeField[x, bottomLeft.y - 1] == Area.Type.Building
                       )
                    {
                        canExpand = false;
                        break;
                    }
                }

                if (canExpand)
                {
                    bottomLeft.y--;
                }
                else
                {
                    yNeg = false;
                }
            }

            //Left
            if (xNeg)
            {
                bool canExpand = true;
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    if (
                            !InBounds(bottomLeft.x - 1, y) ||
                            typeField[bottomLeft.x - 1, y] == Area.Type.Road ||
                            typeField[bottomLeft.x - 1, y] == Area.Type.Building
                       )
                    {
                        canExpand = false;
                        break;
                    }
                }

                if (canExpand)
                {
                    bottomLeft.x--;
                }
                else
                {
                    xNeg = false;
                }
            }



            //Check size
            Vector2Int size = topRight - bottomLeft + new Vector2Int(1, 1);

            if (size.x >= maxSize)
            {
                xNeg = false;
                xPos = false;
            }
            if (size.y >= maxSize)
            {
                yNeg = false;
                yPos = false;
            }
        }

        Vector2Int rectSize = topRight - bottomLeft + new Vector2Int(1, 1);

        //List all buildings that can fit this rect
        var buildingQuery =
            from set in buildingSet.sets
            from bu in set.buildings
            where bu.buildingSize.x <= rectSize.x && bu.buildingSize.y <= rectSize.y
            group bu by set.buildingType;

        foreach(var typeGroup in buildingQuery)
        {
            foreach(BuildingSet.Building b in typeGroup)
            {

            }
        }

        //Fit a random building onto rectangle
        int rotation = 0;

        if(typeField[position.x + 1, position.y] == Area.Type.Road)
        {
            rotation = 1;
        } 
        else if(typeField[position.x - 1, position.y] == Area.Type.Road)
        {
            rotation = 3;
        }
        else if(typeField[position.x, position.y + 1] == Area.Type.Road)
        {
            rotation = 2;
        }
        /*
        else if(typeField[position.x, position.y - 1] == Area.Type.Road)
        {
            rotation = 0;
        }
        */
                            
        

        //Mark area for building
        for (int y = bottomLeft.y; y <= topRight.y; y++)
        {
            for (int x = bottomLeft.x; x <= topRight.x; x++)
            {
                typeField[x, y] = Area.Type.Building;
            }
        }

        building.debug = -1;
        building.debugScale = rectSize;
        building.position = ((Vector2)(bottomLeft + topRight)) / 2f;
        return building;
    }



    //Shoots 4 rays in all directions and checks if 'type' is hit on all sides
    bool IsEnclosedBy(Vector2Int start, Area.Type type)
    {
        bool isEnclosed = true;

        Vector2Int direction = Vector2Int.zero;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    direction = new Vector2Int(1, 0);
                    break;
                case 1:
                    direction = new Vector2Int(-1, 0);
                    break;
                case 2:
                    direction = new Vector2Int(0, 1);
                    break;
                case 3:
                    direction = new Vector2Int(0, -1);
                    break;
            }

            Vector2Int position = start;

            bool hitType = false;
            for (int l = 0; l < areaSize.x; l++)
            {
                if (
                    position.x >= bottomLeftBoundingPoint.x &&
                    position.x < topRightBoundingPoint.x &&
                    position.y >= bottomLeftBoundingPoint.y &&
                    position.y < topRightBoundingPoint.y
                  )
                {
                    if (typeField[position.x, position.y] == type)
                    {
                        hitType = true;
                        break;
                    }
                }
                else
                {
                    break;
                }

                position += direction;
            }

            if (!hitType)
            {
                isEnclosed = false;
                break;
            }
        }

        return isEnclosed;
    }

    void FloodFill(Vector2Int start)
    {
        bool[,] arr = new bool[areaSize.x, areaSize.y];

        List<Vector2Int> positionsToCheck = new List<Vector2Int>();
        positionsToCheck.Add(start);

        int startIndex = 0;

        while (true)
        {
            int len = positionsToCheck.Count;

            bool foundNew = false;
            for (int i = startIndex; i < len; i++)
            {
                Vector2Int v = positionsToCheck[i];
                arr[v.x, v.y] = true;
                if (typeField[v.x, v.y] == Area.Type.Empty)
                {
                    //If we hit the city bounding box, stop floodFillind, this is an error
                    if (!InBounds(v))
                    {
                        Debug.Log("Error FloodFilling area!");
                        break;
                    }

                    typeField[v.x, v.y] = Area.Type.City;
                    foundNew = true;

                    if (!arr[v.x + 1, v.y])
                    {
                        positionsToCheck.Add(new Vector2Int(v.x + 1, v.y));
                    }

                    if (!arr[v.x - 1, v.y])
                    {
                        positionsToCheck.Add(new Vector2Int(v.x - 1, v.y));
                    }

                    if (!arr[v.x, v.y + 1])
                    {
                        positionsToCheck.Add(new Vector2Int(v.x, v.y + 1));
                    }

                    if (!arr[v.x, v.y - 1])
                    {
                        positionsToCheck.Add(new Vector2Int(v.x, v.y - 1));
                    }
                }
            }

            if (!foundNew)
            {
                break;
            }

            startIndex = len;
        }
    }
}