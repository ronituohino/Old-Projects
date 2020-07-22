[System.Serializable]
public class Order
{
    public int id;

    public float price;
    public float time;

    public LiquidMix drink;

    public Order(int id, float price, float time, LiquidMix drink)
    {
        this.id = id;
        this.price = price;
        this.time = time;
        this.drink = drink;
    }
}
