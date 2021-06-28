using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator animator;

    List<Transform> liveEntitiesInRange = new List<Transform>();
    public bool doorOpen = false;
    public bool lockDoor = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            liveEntitiesInRange.Add(collision.transform);
            if (!doorOpen)
            {
                OpenDoor();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            liveEntitiesInRange.Remove(collision.transform);
            if (liveEntitiesInRange.Count <= 0 && doorOpen)
            {
                CloseDoor();
            }
        }
    }

    void OpenDoor()
    {
        if (!lockDoor)
        {
            animator.SetBool("Opened", true);
            doorOpen = true;
        }
    }

    void CloseDoor()
    {
        if (!lockDoor)
        {
            animator.SetBool("Opened", false);
            doorOpen = false;
        }
    }
}
