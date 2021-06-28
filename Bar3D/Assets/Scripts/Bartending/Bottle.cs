using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bartending/Bottle")]
public class Bottle : BartendingItem
{
    public FluidType fluidType;
    
    //Sorted by their types
    public enum FluidType
    {
        //https://manlymanners.wordpress.com/2016/04/11/fermentation-vs-brewing-vs-distillation-the-delicious-distinctions/
        
        //MADE BY BREWING
        //Beer
        Ale, //Fermented at the top
        Lager, // -,,- bottom
        
        
        
        //MADE BY FERMENTING
        //Wines
        //Grape based fermented drink
        //https://en.wikipedia.org/wiki/Wine
        Red_wine,
        White_wine,
        Rosé,
        Sparkling_wine,
        
        Fortified_wine, //Vermouth and Port https://en.wikipedia.org/wiki/Fortified_wine
        
        //Honey based fermented drink, made almost like wine
        Mead, //https://en.wikipedia.org/wiki/Mead
        
        //Apple based fermented drink
        Cider, //https://en.wikipedia.org/wiki/Cider
        
        
        
        //MADE BY DISTILLING
        //Liquors, the 6 bases for cocktails, made from different ingredients
        //https://www.thespruceeats.com/quick-guide-to-distilled-spirits-760713
        Brandy, //https://en.wikipedia.org/wiki/Brandy
        Gin, //https://en.wikipedia.org/wiki/Gin
        Rum, //https://en.wikipedia.org/wiki/Rum
        Tequila, //https://en.wikipedia.org/wiki/Tequila
        Vodka, //https://en.wikipedia.org/wiki/Vodka
        Whisky, //https://en.wikipedia.org/wiki/Whisky
        
        //A rice based spirit
        //A Japan themed site
        Sake, //https://en.wikipedia.org/wiki/Sake
        
        //A specific type of spirit
        //A hallucinogenic website maybe?
        //https://vinepair.com/spirits-101/intro-absinthe-guide/
        Absinthe,
    }
    
    public Color fluidColor;
    
    //Returns bottleType that replaces _ with 'space'
    public static string CleanType(FluidType type)
    {
        return Enum.GetName(typeof(FluidType), type).Replace('_', ' ');
    }
    
    [Tooltip("In liters")]
    public float capacity;
    
    [Tooltip("The point at which fluid is spawned")]
    public Vector3 bottleFluidPoint;
    
    [Tooltip("The amount of units poured per second")]
    public float pourRatePerSecond;
}