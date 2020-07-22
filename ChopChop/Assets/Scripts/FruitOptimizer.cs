using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add this to fruit to optimize them after they are thrown out

public class FruitOptimizer : Singleton<FruitOptimizer>
{
    public List<FollowedFruit> followedFruits = new List<FollowedFruit>();

    private void FixedUpdate()
    {
        int count = followedFruits.Count;
        //Check if fruits are on the floor and remove them
        for (int i = 0; i < count; i++)
        {
            Rigidbody r = followedFruits[i].rb;
            if (r.centerOfMass.y < FruitHandler.Instance.floorPlane.transform.position.y)
            {
                //Apply some cool material that goes like fades away or something
                GameObject obj = followedFruits[i].obj;
                obj.transform.parent = SlicingScript.Instance.destroyParent.transform;
                RemoveFromMovingFruits(obj);
                followedFruits.RemoveAt(i);

                Destroy(obj);
                break;
            }
        }
    }

    void RemoveFromMovingFruits(GameObject fruit)
    {
        int count = FruitHandler.Instance.movingFruits.Count;
        for(int i = 0; i < count; i++)
        {
            if(FruitHandler.Instance.movingFruits[i].fruit == fruit)
            {
                FruitHandler.Instance.movingFruits.RemoveAt(i);
                break;
            }
        }
    }

    //Stores also the rigidbody of the object reducing load
    public struct FollowedFruit
    {
        public GameObject obj;
        public Rigidbody rb;

        public FollowedFruit(GameObject obj, Rigidbody rb)
        {
            this.obj = obj;
            this.rb = rb;
        }
    }
}
