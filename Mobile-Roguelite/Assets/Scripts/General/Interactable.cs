using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public int priority;

    public abstract void Enter(int fingerId); // Called when a finger enters a behaviour
    public abstract void Stay(Vector2 delta); // Called when a finger stays on a behaviour
    //public void Exit(); // Called when a finger exits a behaviour


    // Called when a finger is lifted
    public abstract void Complete(); // Called on behaviours on which a finger is first lifted
    public abstract void Interrupt(); // Called on ALL behaviours that were pressed after any finger is lifted
}