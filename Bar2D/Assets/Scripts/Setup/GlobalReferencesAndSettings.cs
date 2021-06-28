using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GlobalReferencesAndSettings : Singleton<GlobalReferencesAndSettings>
{
    public WaitForFixedUpdate wait;

    [Header("Physics Objects Settings")]

    public GameObject spriteInstance;
    public GameObject shadowObjectInstance;
    public GameObject glassFullnessInstance;

    [Space]

    public GameObject fluidSpawnerInstance;
    public GameObject pourTargetCrossInstance;

    [Space]

    public float objectHoverHeight;
    public float groundHitForceMultiplier;

    [Space]

    public float objectMovementMultiplier;
    public float handGrabDistance;

    [Space]

    public float rotationMultiplier;
    public Vector2 rotationLimits;

    public Material glassFillMaterial;

    [Header("NPC Settings")]

    public float probabilityIncreaseForGoingToService = 0.05f;

    [Header("Bottle Info Settings")]

    public float infoOpenSpeedMultiplier;
    public float textMaskOffset;

    public FullnessText[] fullnessTexts;

    [System.Serializable]
    public struct FullnessText
    {
        public string text;

        [Range(0, 1f)]
        public float fullness;
    }

    [Header("Pour Settings")]

    public float pourRange;
    public AnimationCurve pourCurve;

    [Header("Main Scene References")]

    public Ship ship = null;
    public Navigation navigation = null;

    [Space]

    public MoneyManager moneyManager = null;
    public BartendingBook bartendingBook = null;
    public FluidManager fluidManager = null;

    [Space]

    public Transform playersParent;
    public Transform physicsObjectsParent;

    [Space]

    public NavMeshSurface2d navMesh;

    private void Awake()
    {
        wait = new WaitForFixedUpdate();
        DontDestroyOnLoad(gameObject);
    }
}