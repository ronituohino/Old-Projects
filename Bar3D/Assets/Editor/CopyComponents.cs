using UnityEngine;
using UnityEditor;
using System.Collections;

// An editor script to help update prefabs
public class CopyComponents : ScriptableWizard
{
    public GameObject[] fromObjects;
    public GameObject[] toObjects;

    [MenuItem("Custom/Copy Components")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CopyComponents>("Copy Components", "Copy");
    }

    void OnWizardCreate()
    {
        CopyObject(fromObjects, toObjects);
    }

    void CopyObject(GameObject[] fromObjects, GameObject[] toObjects)
    {
        int len = fromObjects.Length;
        if(len == toObjects.Length)
        {
            for(int i = 0; i < len; i++)
            {
                GameObject from = fromObjects[i];
                GameObject to = toObjects[i];

                Component[] fromComps = from.GetComponents<Component>();
                foreach(Component c in fromComps)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(c);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(to.gameObject);
                }
            }
        }
        else
        {
            Debug.Log("Object child count does not match");
            return; 
        }
    }
}