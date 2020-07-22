using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BLINDED_AM_ME;

public class SlicingScript : Singleton<SlicingScript>
{
    public GameObject cuttingPlane;
    public GameObject sliceParent;
    public GameObject destroyParent;

    Vector3 bump = new Vector3(0.005f, 0, 0);

    public void SplitMesh()
    {
        if (CollisionDetection.Instance.collidingWithFruit)
        {
            int count = FruitHandler.Instance.movingFruits.Count;
            for (int i = 0; i < count; i++)
            {
                FruitHandler.FruitMoving fm = FruitHandler.Instance.movingFruits[i];

                GameObject[] subFruits = GetSubfruits(fm.fruit.transform);
                int childCount = subFruits.Length;

                GameObject newFruit = fm.fruit;
                GameObject slice = null;

                //Cut the main fruit and the objects inside it
                GameObject[] newObjects = MeshCut.Cut(fm.fruit, cuttingPlane.transform.position, -cuttingPlane.transform.forward, fm.cutMat);
                if (newObjects != null)
                {
                    newFruit = newObjects[0];
                    MeshCollider fruitMeshCollider = newFruit.GetComponent<MeshCollider>();
                    MeshFilter fruitMeshFilter = newFruit.GetComponent<MeshFilter>();

                    fruitMeshCollider.sharedMesh = fruitMeshFilter.mesh;

                    slice = newObjects[1];
                    MakePhysicsObject(slice);

                    for (int c = 0; c < childCount; c++)
                    {
                        if (!fm.subFruits[c].external)
                        {
                            //Cut child objects inside
                            GameObject[] newObjects2 = MeshCut.Cut(subFruits[c], cuttingPlane.transform.position, -cuttingPlane.transform.forward, fm.subFruits[c].subMaterial);

                            if (newObjects2 != null)
                            {
                                GameObject o1 = newObjects2[0];
                                GameObject o2 = newObjects2[1];

                                o1.transform.parent = newFruit.transform;
                                o2.transform.parent = slice.transform;

                                if (fm.subFruits[c].extendFaces)
                                {
                                    o1.transform.localScale = o1.transform.localScale + bump;
                                    o1.transform.localPosition = o1.transform.localPosition + 2 * bump;
                                    o2.transform.localPosition = o2.transform.localPosition - bump;
                                }
                            }
                            else
                            {
                                //Transfer the children to the slice if the child object is past the knife
                                GameObject obj = subFruits[c];
                                if (FruitHandler.Instance.PastKnife(obj.GetComponent<MeshFilter>().mesh.vertices, newFruit.transform))
                                {
                                    obj.transform.parent = slice.transform;
                                    
                                }
                            }
                        }
                    }

                    //Update the movingFruit part
                    FruitHandler.Instance.movingFruits[i] = new FruitHandler.FruitMoving(newFruit, fm.rb, fruitMeshFilter, fm.cutMat,  fm.subFruits, true);
                }

                //Cut any extensions that are also children of the main object
                for (int c = 0; c < childCount; c++)
                {
                    if (fm.subFruits[c].external)
                    {
                        //Cut child objects inside
                        GameObject[] newObjects3 = MeshCut.Cut(subFruits[c], cuttingPlane.transform.position, -cuttingPlane.transform.forward, fm.subFruits[c].subMaterial);

                        if (newObjects3 != null)
                        {
                            GameObject o1 = newObjects3[0];
                            GameObject o2 = newObjects3[1];

                            o1.transform.parent = newFruit.transform;
                            o1.GetComponent<MeshCollider>().sharedMesh = o1.GetComponent<MeshFilter>().mesh;

                            
                            if (slice == null)
                            {
                                MakePhysicsObject(o2);
                            }
                            //If we sliced the main fruit, make this a child of that
                            else
                            {
                                o2.transform.parent = slice.transform;
                            }
                        }
                        else
                        {
                            //Transfer the children to the slice if the child object is past the knife
                            GameObject obj = subFruits[c];
                            if (FruitHandler.Instance.PastKnife(obj.GetComponent<MeshFilter>().mesh.vertices, obj.transform))
                            {
                                if (slice != null)
                                {
                                    obj.transform.parent = slice.transform;
                                }
                                else
                                {
                                    MakePhysicsObject(obj);
                                }
                            }
                        }
                    }
                }
            }
        }

        GarbageCollector.Instance.CollectInOneFrame();
    }

    //Makes gameObject g a physics object, like a slice
    private void MakePhysicsObject(GameObject g)
    {
        g.transform.parent = sliceParent.transform;
        MeshFilter sliceMeshFilter = g.GetComponent<MeshFilter>();
        Rigidbody rb = g.AddComponent<Rigidbody>();
        rb.mass = VolumeOfMesh(sliceMeshFilter.mesh);
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        MeshCollider sliceMeshCollider = g.AddComponent<MeshCollider>();
        sliceMeshCollider.sharedMesh = sliceMeshFilter.mesh;
        sliceMeshCollider.convex = true;

        FruitOptimizer.Instance.followedFruits.Add(new FruitOptimizer.FollowedFruit(g, rb));
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int length = mesh.triangles.Length;

        for (int i = 0; i < length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    readonly List<GameObject> objects = new List<GameObject>();

    public GameObject[] GetSubfruits(Transform parent)
    {
        objects.Clear();
        foreach(Transform t1 in parent)
        {
            if(t1.tag == "Subfruit")
            {
                objects.Add(t1.gameObject);
            } else
            {
                foreach (Transform t2 in t1)
                {
                    if (t2.tag == "Subfruit")
                    {
                        objects.Add(t2.gameObject);
                    }
                }
            }
        }
        return objects.ToArray();
    }
}
