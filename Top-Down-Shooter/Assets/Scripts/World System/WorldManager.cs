//Handles level loading and contains data on where we currently are
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : Singleton<WorldManager>
{
    public World gameMap;
    public Area currentArea;

    [SerializeField] Animator animator;

    const string FADE_IN = "In";
    const string FADE_OUT = "Out";

    private void Awake()
    {
        gameMap = WorldMapGenerator.Instance.GenerateWorld();
    }

    public delegate void WordlLoadedEvent();
    public event WordlLoadedEvent WorldLoaded = delegate { };

    //Loads a new area in the game
    public void LoadArea(Area areaToLoad)
    {
        print($"Loading: {areaToLoad.seed}");

        animator.SetTrigger(FADE_IN);

        Scene s = SceneManager.CreateScene(areaToLoad.name, new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        SceneManager.SetActiveScene(s);

        GameObject plotParent = new GameObject("Plots");

        int buildingCount = areaToLoad.buildings.Count;

        for (int i = 0; i < buildingCount; i++)
        {
            Area.Building b = areaToLoad.buildings[i];

            if (b.debug == -1)
            {
                Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                GameObject g = new GameObject();
                g.transform.parent = plotParent.transform;
                g.transform.position = b.position;
                g.transform.localScale = b.debugScale;

                SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
                sr.color = c;
                sr.sprite = AreaGenerator.Instance.debugTile;
            }
            else if (b.debug == 1)
            {
                Instantiate(AreaGenerator.Instance.debugTiles[b.debug], b.position, Quaternion.identity, plotParent.transform);
            }
            else
            {
                /*Instantiate
                           (
                               b.gameObject,

                               b.position,
                               Quaternion.Euler(0, 0, 90 * b.rotation),

                               plotParent.transform
                           );*/
            }
        }

        currentArea = areaToLoad;

        animator.SetTrigger(FADE_OUT);
        WorldLoaded?.Invoke();
    }
}