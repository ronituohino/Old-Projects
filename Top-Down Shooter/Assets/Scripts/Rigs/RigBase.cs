using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigBase : MonoBehaviour
{
    public List<SpritePhysicPair> pairs = new List<SpritePhysicPair>();

    [Space]

    public float dampening;
    public float rotationLerp;

    [System.Serializable]
    public struct SpritePhysicPair
    {
        public bool lerpPosition;
        public bool lerpRotation;
        public Transform physicsTransform;
        public Transform spriteTransform;
    }

    public float movementSpeed;
    public float rotationSpeed;
}