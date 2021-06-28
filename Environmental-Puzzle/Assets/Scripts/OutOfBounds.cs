using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Destroy(other);
    }
}