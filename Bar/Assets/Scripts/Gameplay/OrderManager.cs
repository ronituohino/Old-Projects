using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Keeps track of the orders the player receives
public class OrderManager : Singleton<OrderManager>
{
    public List<Order> orders = new List<Order>();

    public float mixChance;

    public void AddOrder(Order o)
    {
        orders.Add(o);
    }

    void RemoveOrder(Order o)
    {
        orders.Remove(o);
    }

    public void CompleteOrder(Order o)
    {
        Statistics.Instance.balance += o.price;
        RemoveOrder(o);
    }

    public void FailOrder(Order o)
    {
        RemoveOrder(o);
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        //int len = ;
        for (int i = 0; i < orders.Count; i++)
        {
            Order o = orders[i];
            o.time -= delta;

            if (o.time <= 0)
            {
                FailOrder(o);
            }
        }
    }

    //Single
    public Order GenerateOrder()
    {
        bool mix = Random.value < mixChance;
        LiquidMix drink = new LiquidMix(new List<Material>(), new List<float>());

        if (mix)
        {
            int amountOfLiquids = Random.Range(2, 4); //2 or 3

            bool finished = false;
            int amount = LiquidList.Instance.amount;
            while (!finished)
            {
                Material l = LiquidList.Instance.liquids[Random.Range(0, amount)];
                if (!drink.liquids.Contains(l))
                {
                    drink.liquids.Add(l);
                    if (drink.liquids.Count >= amountOfLiquids)
                    {
                        finished = true;
                    }
                }
            }

            List<float> ratios = new List<float>();
            float total = 0f;

            for (int r = 0; r < amountOfLiquids; r++)
            {
                if (r == amountOfLiquids - 1) //Last ratio
                {
                    ratios.Add(1 - total);
                }
                else if (r > 0) //Middle ratio
                {
                    ratios.Add(Random.Range(0, ratios[r - 1] * 0.75f));
                    total += ratios[r];
                }
                else //First
                {
                    ratios.Add(Random.Range(0.33f, 0.5f));
                    total += ratios[r];
                }
            }

            drink.ratios = ratios;
        }
        else
        {
            Material l = LiquidList.Instance.liquids[Random.Range(0, LiquidList.Instance.amount)];
            drink.liquids.Add(l);

            drink.ratios.Add(1f);
        }

        return new Order(Statistics.Instance.ordersReceived, 3f, Random.Range(30, 60), drink); ;
    }

    //Multiple
    public Order[] GenerateOrder(int orderAmount)
    {
        Order[] orders = new Order[orderAmount];

        for (int i = 0; i < orderAmount; i++)
        {
            orders[i] = GenerateOrder();
        }

        return orders;
    }
}
