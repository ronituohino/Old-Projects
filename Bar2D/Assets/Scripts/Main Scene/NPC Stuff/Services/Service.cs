using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for other classes that offer services to npc's

// Bars, Dance floors, food stuff, all sorts of stuff that
// need them to be at some point and do some task!
public abstract class Service : MonoBehaviour
{
    public abstract Transform ReserveRandomServicePoint(NPC npc);

    public abstract bool HasFreeSpace();

    public abstract void Arrive(NPC npc, Transform point);

    internal abstract void Release(NPC npc, Transform point);
}