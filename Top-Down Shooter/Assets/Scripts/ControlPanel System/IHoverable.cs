//Attached to UI elements
public interface IHoverable
{
    //Show item extra info, lighten selection boxes
    void OnHoverEnter();

    void OnHover();

    //Hide extra info, darken selectables
    void OnHoverExit();
}