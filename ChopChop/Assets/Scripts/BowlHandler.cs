using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlHandler : Singleton<BowlHandler>
{
    public GameObject bowlPrefab;
    Animator animator;
    public float fruitAmountMinimum;

    private void Start()
    {
        SpawnNewBowl();
    }

    public struct Bowl
    {
        public GameObject bowlObject;
        public Rigidbody[] fruits;
        public float amountOfFruit;

        public Bowl(GameObject bowlObject, Rigidbody[] fruits, float amountOfFruit)
        {
            this.bowlObject = bowlObject;
            this.fruits = fruits;
            this.amountOfFruit = amountOfFruit;
        }
    }

    public Bowl currentBowl;
    public Queue<Bowl> bowlLine = new Queue<Bowl>();

    readonly List<Rigidbody> fruitInBowl = new List<Rigidbody>();

    //Also stores the fruit objects as an array (rigidbodies)
    float CalculateFruitInCurrentBowl()
    {
        fruitInBowl.Clear();
        float sum = 0f;

        int amountOfFruit = FruitOptimizer.Instance.followedFruits.Count;
        for(int i = 0; i < amountOfFruit; i++)
        {
            GameObject obj = FruitOptimizer.Instance.followedFruits[i].obj;
            Rigidbody rb = FruitOptimizer.Instance.followedFruits[i].rb;

            if(obj.transform.position.x > FruitHandler.Instance.xCuttingPlane && rb.IsSleeping())
            {
                sum += FruitOptimizer.Instance.followedFruits[i].rb.mass;
                fruitInBowl.Add(rb);
            }
        }

        currentBowl.fruits = fruitInBowl.ToArray();
        return sum;
    }

    public void SpawnNewBowl()
    {
        currentBowl = new Bowl(Instantiate(bowlPrefab, transform), null, 0f);
        animator = currentBowl.bowlObject.GetComponent<Animator>();
    }
    
    public void GiveBowl()
    {
        currentBowl.amountOfFruit = CalculateFruitInCurrentBowl();
        //Debug.Log(currentBowl.amountOfFruit);

        //Check if there is enough fruit in the bowl to give to the customer
        if(currentBowl.amountOfFruit >= fruitAmountMinimum)
        {
            int count = currentBowl.fruits.Length;
            for(int i = 0; i < count; i++)
            {
                currentBowl.fruits[i].transform.parent = currentBowl.bowlObject.transform;
                Destroy(currentBowl.fruits[i]);
            }

            animator.SetTrigger("Serve");
        }
    }
}
