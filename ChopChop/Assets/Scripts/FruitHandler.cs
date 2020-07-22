using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Moves fruit 

public class FruitHandler : Singleton<FruitHandler>
{
    public GameObject cuttingPlane;
    public GameObject floorPlane;

    public Vector3 movingSpeed;

    public List<Vector3> customSpawnLocations = new List<Vector3>();

    [Space]

    public Vector3 cutLastPieceThrowingSpeed;
    public Vector3 unCutLastPieceThrowingSpeed;

    [Space]

    //Public list of all fruits in the game
    public List<Fruit> fruits = new List<Fruit>();

    [Space]

    //Public list of all the fruits rolling down
    [HideInInspector]
    public List<FruitMoving> movingFruits = new List<FruitMoving>();

    [Space]

    //Fruit auto-spawn variables
    public bool spawnFruits;
    public float spawnSpeed;
    float timer = 0f;

    GameObject fruit = null;
    Rigidbody rb;
    MeshFilter mf;
    //Vector3 furthestVertexDiff;
    [HideInInspector]
    public float xCuttingPlane;

    int fruitNum = 0;
    int fruitLayer;

    private void Start()
    {
        xCuttingPlane = cuttingPlane.transform.position.x;
        fruitLayer = LayerMask.NameToLayer("Fruit");
    }

    public void SpawnFruit(Fruit fruitToSpawn)
    {
        //Spawn the fruit object
        if(customSpawnLocations.Count > 0 && fruitNum < customSpawnLocations.Count)
        {
            fruit = Instantiate(fruitToSpawn.prefab, customSpawnLocations[fruitNum], Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)), transform);
        } else
        {
            fruit = Instantiate(fruitToSpawn.prefab, transform.position, Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)), transform);
        }
        
        fruit.name = fruitNum.ToString() + "-" + fruitToSpawn.id.ToString();

        mf = fruit.GetComponent<MeshFilter>();
        rb = fruit.GetComponent<Rigidbody>();

        //Set the fruit mass to something crazy so that it pushes stuff away from it
        rb.mass = 100f;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = movingSpeed;

        //SlicingScript.Instance.fruitObject = fruit;
        //SlicingScript.Instance.fruit = fruitToSpawn;

        fruit.layer = fruitLayer;
        GameObject[] subFruits = SlicingScript.Instance.GetSubfruits(fruit.transform);
        int childCount = subFruits.Length;
        for(int i = 0; i < childCount; i++)
        {
            subFruits[i].layer = fruitLayer;
        }

        if (PlatformDetection.Instance.inEditor)
        {
            if(fruit.transform.childCount > 0)
            {
                if(childCount == 0)
                {
                    Debug.LogWarning("Might want to check subFruit tags!");
                }
            }
        }

        //Set the cutting plane to a good distance from the knife so that the cutting looks good
        //cuttingPlane.transform.position = new Vector3(knife.transform.position.x - xPlaneMovementPerSpeedUnit * fruitToSpawn.movingSpeed, cuttingPlane.transform.position.y, cuttingPlane.transform.position.z);
        //xCuttingPlane = cuttingPlane.transform.position.x;

        
        movingFruits.Add(new FruitMoving(fruit, rb, mf, fruitToSpawn.fruitInnerMaterial, fruitToSpawn.fruitSubStuff, false));
        fruitNum++;

        FruitOptimizer.Instance.followedFruits.Add(new FruitOptimizer.FollowedFruit(fruit, rb));

        GarbageCollector.Instance.CollectInOneFrame();
    }

    readonly List<int> markedFruit = new List<int>();

    private void FixedUpdate()
    {
        //Keep track of fruits and throw them away after they pass the knife
        if (movingFruits.Count > 0)
        {
            int i = 0;
            markedFruit.Clear();
            foreach(FruitMoving fm in movingFruits)
            {
                if (PastKnife(fm.mf.mesh.vertices, fm.fruit.transform))
                {
                    bool allPast = true;
                    GameObject[] subFruits = SlicingScript.Instance.GetSubfruits(fm.fruit.transform);
                    foreach(GameObject g in subFruits)
                    {
                        if(!PastKnife(g.GetComponent<MeshFilter>().mesh.vertices, g.transform))
                        {
                            allPast = false;
                        }
                    }

                    if (allPast)
                    {
                        if (fm.hasBeenCut)
                        {
                            fm.rb.velocity = cutLastPieceThrowingSpeed;
                            fm.rb.mass = SlicingScript.Instance.VolumeOfMesh(fm.mf.sharedMesh);

                            fm.fruit.gameObject.layer = 0;
                            fm.fruit.transform.parent = SlicingScript.Instance.sliceParent.transform;
                        }
                        else
                        {
                            fm.rb.mass = 1;
                            fm.rb.velocity = unCutLastPieceThrowingSpeed;

                            fm.fruit.gameObject.layer = 0;
                            fm.fruit.transform.parent = SlicingScript.Instance.destroyParent.transform;
                        }
                        markedFruit.Add(i);
                    }
                }
                i++;
            }

            if(markedFruit.Count > 0)
            {
                int y = 0;
                foreach (int x in markedFruit)
                {
                    movingFruits.RemoveAt(x - y);
                    y++;
                }
            }
        }

        //Auto-Spawning
        if (spawnFruits)
        {
            timer += Time.deltaTime;
            if(timer >= spawnSpeed)
            {
                SpawnRandomFruit();
                timer = 0f;
            }
        }
    }

    public bool PastKnife(Vector3[] verticies, Transform objectTransform)
    {
        //Vector3[] originalVerts = fm.mf.mesh.vertices;
        int length = verticies.Length;
        for (int i = 0; i < length; i++)
        {
            Vector3 vertex = objectTransform.TransformPoint(verticies[i]);
            if (vertex.x < xCuttingPlane)
            {
                return false;
            }
        }
        return true;
    }

    public void SpawnRandomFruit()
    {
        Fruit fruitToSpawn = fruits[Mathf.RoundToInt(Mathf.Floor(Random.Range(0, fruits.Count)))];
        SpawnFruit(fruitToSpawn);
    }

    public class FruitMoving
    {
        public GameObject fruit;
        public Rigidbody rb;
        public MeshFilter mf;

        public Material cutMat;

        public SubFruit[] subFruits;

        public bool hasBeenCut;

        public FruitMoving(GameObject fruit, Rigidbody rb, MeshFilter mf, Material cutMat, SubFruit[] subFruit, bool hasBeenCut)
        {
            this.fruit = fruit;
            this.rb = rb;
            this.mf = mf;
            this.cutMat = cutMat;
            this.subFruits = subFruit;
            this.hasBeenCut = hasBeenCut;
        }
    }
}
