using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Presents map data
public class MapWindow : Window
{
    [SerializeField] GameObject mapPointMarker;

    [SerializeField] RectTransform mapHandleRect;
    [SerializeField] MapHandle mapHandle;

    [SerializeField] RectTransform playerMarker;

    //Defined in Area.cs, Area.AreaTypes
    //Set sprites for each AreaType in order
    public Sprite[] areaTypeSprites;

    public Marker markerHeading { get; private set; } = null;

    bool traveling = false;
    Vector2 playerVel = Vector2.zero;

    [SerializeField] float playerSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        CreateMapWindow();
    }

    private void Update()
    {
        if(traveling)
        {
            playerMarker.anchoredPosition = Vector2.MoveTowards
                                                (
                                                    playerMarker.anchoredPosition, 
                                                    markerHeading.Rect.anchoredPosition, 
                                                    playerSpeed
                                                );
            mapHandle.SetTargetPosition(-playerMarker.anchoredPosition);

            // Check if we have arrived
            if(Vector2.Distance(playerMarker.anchoredPosition, markerHeading.Rect.anchoredPosition) < 0.1f)
            {
                traveling = false;
                WorldManager.Instance.WorldLoaded += EndTravel;
                WorldManager.Instance.LoadArea(WorldManager.Instance.gameMap.pointsOfInterest[markerHeading.Index].area);
            }
        }
    }

    void CreateMapWindow()
    {
        int markerCount = WorldManager.Instance.gameMap.pointsOfInterest.Count;
        for (int i = 0; i < markerCount; i++)
        {
            World.PointOfInterest pointOfInterest = WorldManager.Instance.gameMap.pointsOfInterest[i];

            // Create map markers for points of interest
            Vector2 posInWindow = pointOfInterest.position - (WorldMapGenerator.Instance.mapDimensions / 2f);
                GameObject g = Instantiate
                                (
                                    mapPointMarker,
                                    mapHandleRect
                                );

                RectTransform rect = g.GetComponent<RectTransform>();
                rect.anchoredPosition = posInWindow;

                Marker m = g.GetComponent<Marker>();
            m.Index = i;
            m.Rect = rect;
                m.MapWindow = this;
                m.markerName = pointOfInterest.area.name;
                g.name = pointOfInterest.area.name;
        }
    }

    public void SetHeading(Marker m)
    {
        markerHeading = m;
    }

    public void RemoveHeading()
    {
        markerHeading = null;
    }

    public void StartTravel()
    {
        InputManager.Instance.IgnoreAllInput = true;
        traveling = true;
    }

    public void EndTravel()
    {
        WorldManager.Instance.WorldLoaded -= EndTravel;
        InputManager.Instance.IgnoreAllInput = false;
    }
}
