/*
MIT License

Copyright (c) 2016 Sebastian

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/
//Thanks! :)


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldOfView : MonoBehaviour
{
    public Transform characterTransform;
    public Collider2D thisCollider;

    [Space]

    public float viewAngle;
    public float viewRadius;

    public ContactFilter2D characterFilter;
    public LayerMask defaultLayerMask;

    [HideInInspector]
    List<Collider2D> targetsInViewRadius = new List<Collider2D>();
    public List<Collider2D> visibleTargets = new List<Collider2D>();

    WaitForSeconds wait;
    void Awake()
    {
        wait = new WaitForSeconds(0.5f);
        StartCoroutine(FindTargetsWithDelay());
    }

    public Action OnTargetsScanned;

    IEnumerator FindTargetsWithDelay()
    {
        while (true)
        {
            yield return wait;
            FindVisibleTargets();
            OnTargetsScanned?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(characterTransform.position, viewRadius);
        Gizmos.DrawLine(characterTransform.position, Quaternion.Euler(0, 0, viewAngle / 2) * characterTransform.up * viewRadius + characterTransform.position);
        Gizmos.DrawLine(characterTransform.position, Quaternion.Euler(0, 0, -viewAngle / 2) * characterTransform.up * viewRadius + characterTransform.position);
    }

    void FindVisibleTargets()
    {
        targetsInViewRadius.Clear();
        visibleTargets.Clear();

        int targetAmount = Physics2D.OverlapCircle(characterTransform.position, viewRadius, characterFilter, targetsInViewRadius);

        for (int i = 0; i < targetAmount; i++)
        {
            Collider2D col = targetsInViewRadius[i];
            if(col != thisCollider)
            {
                Vector2 dirToTarget = col.transform.position - characterTransform.position;
                if (Vector2.Angle(characterTransform.up, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = dirToTarget.magnitude;

                    bool hitAnything = Physics2D.Raycast(characterTransform.position, dirToTarget.normalized, dstToTarget, defaultLayerMask);
                    if (!hitAnything)
                    {
                        visibleTargets.Add(col);
                    }
                }
            }
        }
    }
}
