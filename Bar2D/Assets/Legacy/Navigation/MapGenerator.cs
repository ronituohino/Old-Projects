using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Generates a galaxy to fly around
public class MapGenerator : Singleton<MapGenerator>
{
    public int mapLength;

    [Space]

    public Vector2 areaMinMaxLength;
    public float mapHeight;

    [Space]

    public float distanceBetweenPoints;
    public float emptyDistanceFromTopAndBottom;

    [Header("Points of Interest")]

    public Station[] stations;
    public Planet[] planets;

    [Space]

    public AsteroidField[] asteroidFields;


    [Space]

    public List<Faction> factionsInWorld;

    [System.Serializable]
    public class Map
    {
        public int seed;

        // World map consists of a list of areas <- 2D ->
        public Area[] areaList;

        public int mapLength;
        public float totalMapLength;

        public float mapHeight;

        [System.Serializable]
        public class Area
        {
            public Vector2 areaSize;
            public float xOffsetFromStart;

            public class POIData
            {
                public PointOfInterest pointOfInterest;
                public Vector2 position;
            }

            public List<POIData> pointsOfInterest = new List<POIData>();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public Map GenerateMap(int seed)
    {
        Map map = new Map();

        map.seed = seed;
        Random.InitState(seed);

        map.areaList = new Map.Area[mapLength];
        map.mapLength = mapLength;
        map.mapHeight = mapHeight;

        // Generate lengths for each area here, so that we can generate the point list
        float[] areaLengths = new float[mapLength];
        for(int i = 0; i < mapLength; i++)
        {
            areaLengths[i] = Random.Range(areaMinMaxLength.x, areaMinMaxLength.y);
        }
        map.totalMapLength = areaLengths.Sum();

        List<Vector2> allPoints = new PoissonDiscSampler
                                            (
                                                map.totalMapLength,
                                                mapHeight,
                                                distanceBetweenPoints
                                            ).Samples().Where
                                                            (
                                                                p =>
                                                                p.y <= mapHeight - emptyDistanceFromTopAndBottom &&
                                                                p.y >= emptyDistanceFromTopAndBottom
                                                            ).ToList();

        float xDistance = 0f;
        for (int x = 0; x < mapLength; x++)
        {
            Vector2 areaSize = new Vector2(areaLengths[x], mapHeight);
            List<Vector2> areaPoints = allPoints.FindAll
                                                    (
                                                        p =>
                                                        p.x > xDistance &&
                                                        p.x < xDistance + areaSize.x
                                                    );

            Map.Area area = new Map.Area();
            area.areaSize = areaSize;
            area.xOffsetFromStart = xDistance + areaSize.x / 2f;
            GeneratePoints(area, areaPoints);

            map.areaList[x] = area;
            xDistance += areaSize.x;
        }

        return map;
    }

    public void GeneratePoints(Map.Area area, List<Vector2> areaPoints)
    {
        // Make at least one point be a station or a planet
        Map.Area.POIData data = new Map.Area.POIData();

        Vector2 pos = areaPoints[Random.Range(0, areaPoints.Count)];

        Station station = (Station)PickRandom(stations.Cast<PointOfInterest>().ToArray());

        data.pointOfInterest = station;
        data.position = pos;

        area.pointsOfInterest.Add(data);

        areaPoints.Remove(pos);

        // Set others randomly
        foreach (Vector2 v in areaPoints)
        {
            Map.Area.POIData d = new Map.Area.POIData();

            AsteroidField field = (AsteroidField)PickRandom(asteroidFields.Cast<PointOfInterest>().ToArray());
            d.pointOfInterest = field;
            d.position = v;

            area.pointsOfInterest.Add(d);
        }
    }

    PointOfInterest PickRandom(PointOfInterest[] pointsOfInterest)
    {
        float val = Random.Range(0, 1f);
        for (int i = 0; i < pointsOfInterest.Length; i++)
        {
            PointOfInterest poi = pointsOfInterest[i];
            if (i <= poi.mapSpawnProbability)
            {
                return poi;
            }
        }

        return null;
    }
}