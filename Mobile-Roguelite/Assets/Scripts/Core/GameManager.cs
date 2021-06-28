using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Settings & References")]

    public WaitForFixedUpdate wfu;
    public RectTransform mainRect;

    [SerializeField] GameObject[] objectsToToggleOnTransition;

    [System.Serializable]
    public struct Resources
    {
        public int coins;
        public int supplies;
    }

    [Header("Resources")]

    public Resources resources;

    public bool canMove = false;

    private void Start()
    {
        wfu = new WaitForFixedUpdate();

        MapManager.Instance.LoadMap(0);
        canMove = true;
    }

    public void TriggerTile(Tile t)
    {
        if(!t.triggered)
        {
            canMove = false;

            switch (t.tileType)
            {
                default: //TileType == Empty
                    TileCompleted();
                    break;
                case Tile.TileType.Encounter:
                    EncounterManager.Instance.TransitionTo();
                    break;
                case Tile.TileType.Trap:
                    TileCompleted();
                    break;
                case Tile.TileType.Shop:
                    TileCompleted();
                    break;
                case Tile.TileType.Finish:
                    FinishLevel();
                    break;
            }

            t.triggered = true;
        }
    }

    public void TransitionToMain(Dictionary<int, List<Status>> eventStatuses)
    {
        // Apply all statuses to heroes
        foreach(int i in eventStatuses.Keys)
        {
            PartyManager.Instance.heroes[i].activeStatuses.AddRange(eventStatuses[i]);
            PartyManager.Instance.heroes[i].UpdateHero();
        }

        FadeManager.Instance.Fade(true);
        StartCoroutine(Extensions.AnimationWait(FadeManager.Instance.animator, "FadeIn", TileCompleted));
    }

    public void TileCompleted()
    {
        foreach(GameObject g in objectsToToggleOnTransition)
        {
            g.SetActive(true);
        }

        FadeManager.Instance.Fade(false);
        canMove = true;
    }

    void FinishLevel()
    {
        MapManager.Instance.LoadMap(MapManager.Instance.currentMapIndex + 1);
    }
}