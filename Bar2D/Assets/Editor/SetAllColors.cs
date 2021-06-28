using UnityEngine;
using UnityEditor;

public class SetAllColors : EditorWindow
{
    [SerializeField] private Color color;

    [MenuItem("Tools/Set All Colors")]
    static void CreateSetAllColors()
    {
        EditorWindow.GetWindow<SetAllColors>();
    }

    private void OnGUI()
    {
        color = EditorGUILayout.ColorField(color);

        if (GUILayout.Button("Set All Sprite Renderer Colors"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var spriteRenderers = selected.GetComponentsInChildren<SpriteRenderer>();

                foreach(SpriteRenderer sr in spriteRenderers)
                {
                    sr.color = color;
                }
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}