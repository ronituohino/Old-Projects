//Generates the world map viewed from the map window
using UnityEngine;

public class WorldMapGenerator : Singleton<WorldMapGenerator>
{
    public Vector2 mapDimensions;
    [SerializeField] int seed;

    [SerializeField] float minPointDistance;

    public World GenerateWorld()
    {
        //Reset seed
        if(seed == 0)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        Random.InitState(seed);

        World map = new World();
        map.seed = seed;

        //Generate points in map
        PoissonDiscSampler sampler = new PoissonDiscSampler(mapDimensions.x, mapDimensions.y, minPointDistance);
        var points = sampler.Samples();

        foreach(Vector2 v in points)
        {
            //Generate areas
            int areaSeed = Random.Range(int.MinValue, int.MaxValue);
            Area area = AreaGenerator.Instance.GenerateArea(areaSeed, false);

            //Store area in map
            World.PointOfInterest point = new World.PointOfInterest(area, v);
            map.pointsOfInterest.Add(point);
        }

        return map;
    }
}