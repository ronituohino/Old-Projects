using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// NPC's can order drinks here
public class Bar : Service
{
    [System.Serializable]
    public class BarServicePoint
    {
        public Transform transform;
        public DrinkTriggerArea drinkTrigger;

        public NPC customer;
        public OrderGenerator.Order customerOrder;
    }

    [SerializeField] List<BarServicePoint> points = new List<BarServicePoint>();

    public override Transform ReserveRandomServicePoint(NPC customer)
    {
        int pointCount = points.Count;
        for (int i = 0; i < pointCount; i++)
        {
            BarServicePoint point = points[i];

            if (point.customer == null)
            {
                point.customer = customer;
                return point.transform;
            }
        }

        return null;
    }

    public override bool HasFreeSpace()
    {
        return points.Exists(x => x.customer == null);
    }

    public override void Arrive(NPC customer, Transform point)
    {
        BarServicePoint bsp = points.Find(x => x.transform == point);

        bsp.customerOrder = OrderGenerator.Instance.GenerateOrder();

        customer.ShowDialogue(bsp.customerOrder.ToString(bsp.customerOrder));
    }

    internal override void Release(NPC customer, Transform point)
    {
        BarServicePoint bsp = points.Find(x => x.transform == point);
        bsp.customer = null;

        customer.ReleaseNPCFromService();
    }

    public void CheckIfOrderFilled(DrinkTriggerArea drinkTrigger, List<GlassPhysics> glasses)
    {
        BarServicePoint bsp = points.Find(x => x.drinkTrigger == drinkTrigger);

        Dictionary<int, int> matches = new Dictionary<int, int>();

        int drinkCount = bsp.customerOrder.drinks.Length;
        int glassCount = glasses.Count;

        for(int d = 0; d < drinkCount; d++)
        {
            Drink drink = bsp.customerOrder.drinks[d];
            bool foundMatch = false;

            for(int g = 0; g < glassCount; g++)
            {
                GlassPhysics glass = glasses[g];

                if(!matches.ContainsValue(g) && CheckIfDrinksMatch(drink, glass))
                {
                    matches.Add(d, g);
                    foundMatch = true;
                    break;
                }
            }

            if(!foundMatch)
            {
                return;
            }
        }

        // The order is filled:
        print("Match!");
    }

    bool CheckIfDrinksMatch(Drink drink, GlassPhysics glass)
    {
        Dictionary<int, int> matches = new Dictionary<int, int>();

        int drinkIngredientCount = drink.ingredients.Length;
        int glassContentCount = glass.containedFluids.Count;

        for (int d = 0; d < drinkIngredientCount; d++)
        {
            Drink.Ingredient ingredient = drink.ingredients[d];
            bool foundMatch = false;

            for (int g = 0; g < glassContentCount; g++)
            {
                GlassPhysics.Contents contents = glass.containedFluids[g];

                bool matchingBottle = ingredient.specific ? ingredient.bottle == contents.bottle : ingredient.bottle.fluidType == contents.bottle.fluidType;
                bool matchinRatio = Mathf.Abs(ingredient.ratio - (contents.units / glass.glass.capacity)) <= OrderGenerator.Instance.maxRatioDifference;

                if (!matches.ContainsValue(g) && matchingBottle && matchinRatio)
                {
                    matches.Add(d, g);
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                return false;
            }
        }

        return true;
    }
}