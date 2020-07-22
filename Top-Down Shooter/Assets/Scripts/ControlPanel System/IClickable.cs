//ALL IClickables must also be IHoverables!
interface IClickable
{
    bool disabledClick { get; set; }
    void Click();
}