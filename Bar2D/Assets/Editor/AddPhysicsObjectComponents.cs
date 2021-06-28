using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddPhysicsObjectComponents : EditorWindow
{
    public Object fullness;
    public Object shadow;
    public Object sprite;

    [MenuItem("Tools/Add PhysicsObject Components")]
    static void CreateAddComponents()
    {
        EditorWindow.GetWindow<AddPhysicsObjectComponents>();
    }

    private void OnGUI()
    {
        fullness = EditorGUILayout.ObjectField(fullness, typeof(GameObject), true);
        shadow = EditorGUILayout.ObjectField(shadow, typeof(GameObject), true);
        sprite = EditorGUILayout.ObjectField(sprite, typeof(GameObject), true);

        if (GUILayout.Button("Add PhysicsObject Components"))
        {
            GameObject[] selections = Selection.gameObjects;

            foreach(GameObject g in selections)
            {
                PhysicsObject po = g.GetComponent<PhysicsObject>();
                if (po != null)
                {
                    // Create rigidbody
                    po.rb = g.AddComponent<Rigidbody2D>();
                    po.rb.drag = 5f;
                    po.rb.angularDrag = 1f;
                    po.rb.gravityScale = 0f;
                    po.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                    // Create physics collider
                    po.capsuleCollider2D = g.AddComponent<CapsuleCollider2D>();

                    // Create sprite
                    GameObject spriteObject = Instantiate((GameObject)sprite, g.transform);
                    po.spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();

                    // Configure sprite sorter
                    po.spriteSorter = spriteObject.GetComponent<SpriteSorter>();
                    po.spriteSorter.thisRigidbody = po.rb;

                    // Create click collider
                    if (po.pickable)
                    {
                        po.clickCollider = spriteObject.AddComponent<BoxCollider2D>();
                        po.clickCollider.isTrigger = true;
                    }

                    // Create shadow
                    po.shadowScript = Instantiate((GameObject)shadow, g.transform).GetComponent<Shadow>();
                    po.shadowScript.spriteRenderer = po.spriteRenderer;
                    po.shadowScript.physicsObject = po;
                    po.shadowScript.spriteSorter.connectedSpriteRenderer = po.spriteRenderer;
                    
                    // Glass fullness
                    if (po is GlassPhysics)
                    {
                        GameObject f = Instantiate((GameObject)fullness, g.transform);
                        f.GetComponent<SpriteSorter>().thisRigidbody = po.rb;

                        ((GlassPhysics)po).fullnessSprite = f.GetComponent<SpriteRenderer>();
                    }
                }
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
