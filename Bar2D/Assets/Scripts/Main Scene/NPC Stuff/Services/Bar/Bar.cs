using System.Collections.Generic;
using UnityEngine;

// NPC's can order drinks here
public class Bar : Service
{
    [System.Serializable]
    public class BarServicePoint
    {
        public Transform transform;
        public DrinkTriggerArea drinkTrigger;

        public NPC npc;
        public OrderGenerator.Order npcOrder;
    }

    [SerializeField] List<BarServicePoint> points = new List<BarServicePoint>();

    private void Update()
    {
        // Update times
        foreach(BarServicePoint bsp in points)
        {
            if(bsp.npc != null && bsp.npcOrder != null && bsp.npcOrder.drinks != null && bsp.npcOrder.drinks.Length > 0)
            {
                float newTime = bsp.npcOrder.timeToComplete - Time.deltaTime;
                if (newTime <= 0f)
                {
                    DrinkFail(bsp);
                }
                else
                {
                    bsp.npcOrder.timeToComplete = newTime;
                }
            }
        }
    }

    public override Transform ReserveRandomServicePoint(NPC npc)
    {
        int pointCount = points.Count;
        for (int i = 0; i < pointCount; i++)
        {
            BarServicePoint point = points[i];

            if (point.npc == null)
            {
                point.npc = npc;
                return point.transform;
            }
        }

        return null;
    }

    public override bool HasFreeSpace()
    {
        return points.Exists(x => x.npc == null);
    }

    public override void Arrive(NPC npc, Transform point)
    {
        BarServicePoint bsp = points.Find(x => x.transform == point);
        //bsp.npc is set when the seat is reserved
        bsp.npcOrder = OrderGenerator.Instance.GenerateOrder();

        npc.ShowDialogue(bsp.npcOrder.ToString(bsp.npcOrder));
    }

    internal override void Release(NPC npc, Transform point)
    {
        BarServicePoint bsp = points.Find(x => x.transform == point);
        bsp.npc = null;
        bsp.npcOrder = null;

        npc.ReleaseNPCFromService();
        npc.HideDialogue();
    }

    // The order was filled
    void DrinkPass(BarServicePoint bsp)
    {
        StartCoroutine(GlobalReferencesAndSettings.Instance.moneyManager.SpawnCoins(bsp.npcOrder.price, bsp.transform.position));
        Release(bsp.npc, bsp.transform); // Inefficient, fix if can be fixed later
    }

    // The time ran out and the order was not filled
    void DrinkFail(BarServicePoint bsp)
    {
        Release(bsp.npc, bsp.transform); // Inefficient, fix if can be fixed later
    }

    public void CheckIfOrderFilled(DrinkTriggerArea drinkTrigger, List<GlassPhysics> glasses)
    {
        BarServicePoint bsp = points.Find(x => x.drinkTrigger == drinkTrigger);

        if(bsp.npc != null && bsp.npcOrder != null)
        {
            Dictionary<int, int> matches = new Dictionary<int, int>();

            int drinkCount = bsp.npcOrder.drinks.Length;
            int glassCount = glasses.Count;

            for (int d = 0; d < drinkCount; d++)
            {
                Drink drink = bsp.npcOrder.drinks[d];
                bool foundMatch = false;

                for (int g = 0; g < glassCount; g++)
                {
                    GlassPhysics glass = glasses[g];

                    if (!matches.ContainsValue(g) && CheckIfDrinksMatch(drink, glass))
                    {
                        matches.Add(d, g);
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    return;
                }
            }

            // The order is filled:
            DrinkPass(bsp);
        }
    }

    bool CheckIfDrinksMatch(Drink drink, GlassPhysics glass)
    {
        Dictionary<int, int> matches = new Dictionary<int, int>();

        int drinkIngredientCount = drink.ingredients.Length;
        int glassContentCount = glass.contents.Count;

        for (int d = 0; d < drinkIngredientCount; d++)
        {
            Drink.Ingredient ingredient = drink.ingredients[d];
            bool foundMatch = false;

            for (int g = 0; g < glassContentCount; g++)
            {
                GlassPhysics.Contents contents = glass.contents[g];

                bool matchingBottle = ingredient.specific ? ingredient.bottle == contents.bottle : ingredient.bottle.fluidType == contents.bottle.fluidType;
                bool matchingRatio = Mathf.Abs(ingredient.ratio - (contents.fluidContained / glass.glass.fluidCapacity)) <= OrderGenerator.Instance.maxRatioDifference;

                if (!matches.ContainsValue(g) && matchingBottle && matchingRatio)
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