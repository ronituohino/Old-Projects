using System.Collections.Generic;
using UnityEngine;

public class Raycasting : Singleton<Raycasting>
{
    public Camera cam;

    public List<RaycastHit2D> raycastResults = new List<RaycastHit2D>();
    public int length;

    public ContactFilter2D filter;

    private void FixedUpdate()
    {
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        length = Physics2D.Raycast(r.origin, r.direction, filter, raycastResults);
    }
}
