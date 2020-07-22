using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Fruit Spawn Data")]
public class Fruit : ScriptableObject
{
    public GameObject prefab;
    public Material fruitInnerMaterial;

    [Space]

    public SubFruit[] fruitSubStuff;

    [Space]

    public int id;
}

[System.Serializable]
public class SubFruit
{
    public Material subMaterial;
    public bool external;
    public bool extendFaces;
}
