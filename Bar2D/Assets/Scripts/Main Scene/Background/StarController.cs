using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    public float panMultiplier;
    float xPan = 0f;

    [SerializeField] SpriteRenderer sr;
    Material starMaterial;

    private void Start()
    {
        starMaterial = sr.material;
    }

    // Update is called once per frame
    void Update()
    {
        xPan += panMultiplier * GlobalReferencesAndSettings.Instance.navigation.speed;
        starMaterial.SetVector("_Offset", new Vector4(xPan, 0f, 0f, 0f));
    }
}
