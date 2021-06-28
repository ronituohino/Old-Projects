using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create pairs of transforms and rigidbodies, applies physical transformations and rotations
public class PhysicsTransformPair : MonoBehaviour
{
    [System.Serializable]
    public class Pair
    {
        public Rigidbody rb;
        public Transform tr;

        [Space]

        public bool translate;
        public bool rotate;

        [Space]

        public bool invert;
    }

    public Pair[] pairs;

    [SerializeField] float moveForceMultiplier;
    [SerializeField] float rotationForceMultiplier;

    private void FixedUpdate()
    {
        foreach(Pair p in pairs)
        {
            if(!p.invert)
            {
                if(p.translate)
                {
                    p.rb.velocity = Vector3.zero;
                    p.rb.velocity = (p.tr.position - p.rb.transform.position) * moveForceMultiplier;
                }
                
                if(p.rotate)
                {
                    p.rb.angularVelocity = Vector3.zero;

                    Quaternion current = p.rb.transform.rotation;
                    Quaternion target = p.tr.rotation;
                    Quaternion diff = target * Quaternion.Inverse(current);

                    p.rb.AddTorque(diff.x * rotationForceMultiplier, diff.y * rotationForceMultiplier, diff.z * rotationForceMultiplier, ForceMode.VelocityChange);
                }
            }
            else
            {
                p.tr.position = p.rb.transform.position;
                p.tr.rotation = p.rb.transform.rotation;
            }
        }
    }
}
