using System.Collections;
using UnityEngine;

public class GlobalReferencesAndSettings : Singleton<GlobalReferencesAndSettings>
{
    [Header("Pour settings")]
    public GameObject pourObject;
    public AnimationCurve pourCurve;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}