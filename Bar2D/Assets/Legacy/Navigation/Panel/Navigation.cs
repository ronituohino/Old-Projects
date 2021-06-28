using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The actual 2d panel that is show to the player when navigating
// Handles ship navigation
public class Navigation : ObjectPooler
{
    [Header("References")]

    [SerializeField] CanvasGroup navigationGroup;

    [Space]

    public RectTransform mapRect;
    public RectTransform mapRectInner;

    [Space]

    [HideInInspector]
    public List<Marker> markers = new List<Marker>();
    [HideInInspector]
    public List<Connection> connections = new List<Connection>();

    [Space]

    [HideInInspector]
    public bool opened = false;
    [HideInInspector]
    public bool hovered = false;

    [Space]

    public Slider throttle;
    public RectTransform ship;

    [Space]

    public List<NavigationEvent> navigationEvents = new List<NavigationEvent>();

    [Header("Map Settings")]

    public Vector2 shipOffset;

    [Space]

    [SerializeField] float mapDamp;
    [SerializeField] float mapMovementMultiplier;
    Vector2 vel = Vector2.zero;

    [Space]

    public Sprite unknownSprite;

    [Header("Ship Settings")]

    [HideInInspector]
    public bool autoPilot = false;
    public float sensorRange;

    [HideInInspector]
    public float speed = 0f;
    float dampVel = 0f;
    Connection connectionToFirstMarker;

    [Space]

    public float shipSpeedLimit;
    public float shipAcceleration;
    public float shipRotationLerp;

    [Space]

    Dictionary<int, RectTransform> loadedAreas = new Dictionary<int, RectTransform>();
    RectTransform negativeRect = null;

    Dictionary<Vector2, PointOfInterestComponent> pointsOfInterest = new Dictionary<Vector2, PointOfInterestComponent>();

    public MapGenerator.Map map;

    private void Start()
    {
        map = MapGenerator.Instance.GenerateMap(Mathf.RoundToInt(UnityEngine.Random.Range(int.MinValue, int.MaxValue)));

        mapRect.sizeDelta = new Vector2(map.areaList[map.mapLength - 1].xOffsetFromStart + map.areaList[map.mapLength - 1].areaSize.x, map.mapHeight);
        forceUpdate = true;

        GlobalReferencesAndSettings.Instance.navigation = this;
    }

    public void Open()
    {
        navigationGroup.alpha = 1f;
        navigationGroup.blocksRaycasts = true;

        opened = true;

        InputManager.Instance.LeftMouse += CloseCheck;
    }

    public void Close()
    {
        navigationGroup.alpha = 0f;
        navigationGroup.blocksRaycasts = false;
        opened = false;

        InputManager.Instance.LeftMouse -= CloseCheck;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.LeftMouse -= CloseCheck;
        }
    }

    bool closeCheckStartedOutside = false;
    void CloseCheck(bool pressed)
    {
        if (pressed && !hovered)
        {
            closeCheckStartedOutside = true;
        }
        if (!pressed && !hovered && InputManager.Instance.MouseInScreen && closeCheckStartedOutside)
        {
            Close();
            closeCheckStartedOutside = false;
        }
    }

    bool forceUpdate = false;
    private void Update()
    {
        // Map panning
        if (speed > 0 || forceUpdate)
        {
            mapRect.anchoredPosition = Vector2.SmoothDamp
                                                (
                                                    mapRect.anchoredPosition,
                                                    new Vector2(-ship.anchoredPosition.x - shipOffset.x, 0f),
                                                    ref vel,
                                                    mapDamp
                                                );
        }

        // Handle areas
        if (speed > 0 || forceUpdate)
        {
            List<int> areasToLoad = GetAreasToLoad(GetPositionIndex(ship.anchoredPosition));
            List<int> currentlyLoaded = new List<int>(loadedAreas.Keys);

            // Unload areas
            foreach (int i in currentlyLoaded)
            {
                if (!areasToLoad.Contains(i))
                {
                    UnloadArea(i);
                }
            }

            // Load surronding areas
            foreach (int i in areasToLoad)
            {
                if (!loadedAreas.ContainsKey(i))
                {
                    LoadArea(i);
                }
            }
        }

        // Points of Interest
        if (speed > 0 || forceUpdate)
        {
            foreach (PointOfInterestComponent comp in pointsOfInterest.Values)
            {
                float distance = Vector2.Distance(comp.rect.anchoredPosition, ship.anchoredPosition);

                // Try and sense points of interest
                if (!comp.seen)
                {
                    if (distance - sensorRange - comp.poi.sensorProfile < 0f)
                    {
                        comp.seen = true;

                        Sprite s = comp.poi.mapIcons[UnityEngine.Random.Range(0, comp.poi.mapIcons.Length)];
                        comp.image.sprite = s;
                        comp.rect.sizeDelta = new Vector2(s.texture.width, s.texture.height);
                    }
                }

                // Check if we have any points of interest in trigger range
                if (comp.evnt != null)
                {
                    if (!comp.eventTriggered)
                    {
                        if (distance <= comp.poi.triggerDistance)
                        {
                            if (comp.poi.triggerBehaviour == PointOfInterest.TriggerBehaviour.StopShip)
                            {
                                StartCoroutine(throttle.SetSliderCoroutine(Vector2.zero, 1f));
                            }

                            if (comp.poi.triggerBehaviour == PointOfInterest.TriggerBehaviour.DirectToCenter)
                            {
                                StartCoroutine(throttle.SetSliderCoroutine(Vector2.zero, 1f));
                                autoPilot = true;
                            }

                            comp.evnt?.StartEvent(comp.poi.eventArgs, comp.GetHashCode(), this);
                            comp.eventTriggered = true;
                        }
                    }
                    else
                    {
                        if (distance > comp.poi.triggerDistance)
                        {
                            comp.evnt?.EndEvent();
                            comp.eventTriggered = false;
                        }
                        else
                        {
                            comp.evnt?.UpdateEvent(this, distance);
                        }
                    }
                }
            }
        }

        forceUpdate = false;
    }

    private void FixedUpdate()
    {
        // Ship movement
        // Movement
        float thr = markers.Count > 0 ? throttle.yVal * shipSpeedLimit : 0f;
        if (autoPilot && thr == 0f)
        {
            autoPilot = false;
        }

        speed = Mathf.SmoothDamp(speed, thr, ref dampVel, shipAcceleration, shipSpeedLimit, Time.fixedDeltaTime);

        ship.anchoredPosition += ship.up.ToV2() * speed;

        // Check markers
        if (speed > 0f && markers.Count > 0)
        {
            Vector2 toNextNode = markers[0].rect.anchoredPosition - ship.anchoredPosition;
            float distance = toNextNode.magnitude;

            if (distance < 4f)
            {
                RemoveMarker(0);
            }
            else if (opened && connectionToFirstMarker != null)
            {
                connectionToFirstMarker.UpdateConnection();
            }
        }

        if (markers.Count > 0)
        {
            // Ship connection to first marker
            if (connectionToFirstMarker == null)
            {
                Connection c = Retrieve(3).GetComponent<Connection>();
                c.a = ship;
                c.b = markers[0].rect;

                c.navigationPanel = this;
                c.UpdateConnection();

                connectionToFirstMarker = c;
            }

            // Rotation
            float targetRotation = Extensions.LookAt(ship.anchoredPosition, connectionToFirstMarker.b.anchoredPosition);
            Quaternion rot = Quaternion.Lerp(ship.rotation, Quaternion.Euler(0, 0, targetRotation), shipRotationLerp);
            ship.rotation = rot;
            bool isRotated = Mathf.Abs(targetRotation - rot.eulerAngles.z) < 3f;
        }
    }



    #region Markers & Connections
    public void CreateMarker(Vector2 positionInRect)
    {
        Marker marker = Retrieve(2).GetComponent<Marker>();
        marker.rect.anchoredPosition = positionInRect;
        marker.navigation = this;
        markers.Add(marker);

        int count = markers.Count;
        if (count > 1)
        {
            CreateConnection(count - 2, count - 1);
        }
    }

    public void RemoveMarker(int markerIndex)
    {
        RemoveConnections(markerIndex);

        Return(markers[markerIndex].gameObject);
        markers.RemoveAt(markerIndex);
    }

    void CreateConnection(int aIndex, int bIndex)
    {
        Connection c = Retrieve(3).GetComponent<Connection>();
        c.navigationPanel = this;

        c.a = markers[aIndex].rect;
        c.b = markers[bIndex].rect;

        c.UpdateConnection();
        connections.Add(c);
    }

    void RemoveConnections(int markerIndex)
    {
        Marker m = markers[markerIndex];

        List<Connection> destroyable = connections.FindAll(x => x.a == m.rect || x.b == m.rect);
        int count = destroyable.Count;

        foreach (Connection c in destroyable)
        {
            connections.Remove(c);
            Return(c.gameObject);
        }

        if (markerIndex != 0)
        {
            if (markerIndex != markers.Count - 1)
            {
                CreateConnection(markerIndex - 1, markerIndex + 1);
            }
        }
        else
        {
            if (markers.Count > 1)
            {
                connectionToFirstMarker.b = markers[1].rect;
                connectionToFirstMarker.UpdateConnection();
            }
            else
            {
                Return(connectionToFirstMarker.gameObject);
                connectionToFirstMarker = null;
            }

        }

    }

    public void UpdateConnectionPositions(int markerIndex)
    {
        Marker m = markers[markerIndex];

        foreach (Connection c in connections)
        {
            if (c.a == m.rect || c.b == m.rect)
            {
                c.UpdateConnection();
            }
        }

        if (markerIndex == 0)
        {
            connectionToFirstMarker.UpdateConnection();
        }
    }
    #endregion

    #region Area Loading / Unloading
    int GetPositionIndex(Vector2 anchoredPosition)
    {
        float xSum = 0f;
        for (int i = 0; i < map.mapLength; i++)
        {
            xSum += map.areaList[i].areaSize.x;

            if (xSum > anchoredPosition.x)
            {
                return i;
            }
        }

        print("Error!");
        return -1;
    }

    List<int> GetAreasToLoad(int xPos)
    {
        return new List<int>
        {
            xPos - 1,
            xPos,
            xPos + 1,
            xPos + 2,
            xPos + 3,
        };
    }

    // Area loading / Unloading
    void LoadArea(int index)
    {
        if (index >= 0 && index < map.areaList.Length)
        {
            RectTransform areaRect = Retrieve(0).GetComponent<RectTransform>();

            areaRect.sizeDelta = map.areaList[index].areaSize;

            areaRect.anchoredPosition = new Vector2
                                            (
                                                map.areaList[index].xOffsetFromStart,
                                                0f
                                            );

            loadedAreas.Add(index, areaRect);

            // Load points of interest
            foreach (MapGenerator.Map.Area.POIData data in map.areaList[index].pointsOfInterest)
            {
                PointOfInterestComponent comp = Retrieve(1).GetComponent<PointOfInterestComponent>();
                comp.rect.anchoredPosition = data.position - new Vector2(0f, map.mapHeight / 2f);
                comp.navigation = this;

                comp.poi = data.pointOfInterest;
                comp.seen = false;

                Sprite s = unknownSprite;
                comp.image.sprite = s;
                comp.rect.sizeDelta = new Vector2(s.texture.width, s.texture.height);

                if(!comp.poi.eventName.Equals(""))
                {
                    comp.evnt = navigationEvents.Find(x => x.eventName.Equals(comp.poi.eventName));
                }

                pointsOfInterest.Add(data.position, comp);
            }
        }
        else if (index == -1)
        {
            if (negativeRect == null)
            {
                RectTransform areaRect = Retrieve(0).GetComponent<RectTransform>();

                negativeRect = areaRect;
                negativeRect.sizeDelta = new Vector2(300, map.mapHeight);
                negativeRect.anchoredPosition = new Vector2(-150, 0);
            }
        }
        else
        {
            Debug.LogError("Map ran out!");
        }
    }

    void UnloadArea(int index)
    {
        if (index >= 0)
        {
            Return(loadedAreas[index].gameObject);
            loadedAreas.Remove(index);

            foreach (MapGenerator.Map.Area.POIData data in map.areaList[index].pointsOfInterest)
            {
                Return(pointsOfInterest[data.position].gameObject);
                pointsOfInterest.Remove(data.position);
            }
        }
        else if (index == -1)
        {
            Return(negativeRect.gameObject);
            negativeRect = null;
        }
    }
    #endregion
}