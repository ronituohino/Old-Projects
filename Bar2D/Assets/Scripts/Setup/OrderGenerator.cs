using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Generates orders
public class OrderGenerator : Singleton<OrderGenerator>
{
    public List<Drink> registeredDrinks = new List<Drink>();

    [System.Serializable]
    public class Order
    {
        public Drink[] drinks;
        public float timeToComplete;
        public int price;

        public string ToString(Order o)
        {
            string s = "";
            int drinksLen = o.drinks.Length;
            for (int i = 0; i < drinksLen; i++)
            {
                Drink d = o.drinks[i];

                s += d.name;

                // If not last drink
                if(i < drinksLen - 1)
                {
                    // If second last drink
                    if( i == drinksLen - 2)
                    {
                        s += " and ";
                    }
                    else
                    {
                        s += ", ";
                    }
                }
            }

            return s;
        }
    }

    [Header("Order generation variables")]

    [SerializeField] Vector3 drinkAmountProbabilities;

    [Header("Checking variables")]

    // How much can the drink ratios differ?
    public float maxRatioDifference;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public Order GenerateOrder()
    {
        Order order = new Order();

        int amountOfDrinks = 0;
        float f = Random.Range(0f, 1f);
        if (f < drinkAmountProbabilities.z)
        {
            amountOfDrinks = 3;
        }
        else if (f < drinkAmountProbabilities.y)
        {
            amountOfDrinks = 2;
        }
        else
        {
            amountOfDrinks = 1;
        }

        order.drinks = new Drink[amountOfDrinks];

        for (int i = 0; i < amountOfDrinks; i++)
        {
            Drink d = GenerateDrink();
            d.complexity = CalculateComplexity(d);
            order.drinks[i] = d;
        }

        // Make complex drinks first in array
        order.drinks = order.drinks.OrderBy(x => x.complexity).ToArray();

        order.price = CalculateOrderValue(order);
        order.timeToComplete = CalculateTimeToComplete(order);

        return order;
    }

    Drink GenerateDrink()
    {
        int count = registeredDrinks.Count;
        return registeredDrinks[Random.Range(0, count - 1)];
    }

    float CalculateComplexity(Drink drink)
    {
        float complexity = 0f;
        foreach(Drink.Ingredient ingredient in drink.ingredients)
        {
            if(ingredient.specific)
            {
                complexity += 3f;
            }
            else
            {
                complexity += 1f;
            }
        }

        return complexity;
    }

    // Calculates how much compensation the player gets for completing an order
    int CalculateOrderValue(Order order)
    {
        return 3;
    }

    float CalculateTimeToComplete(Order order)
    {
        return 10f;
    }
}