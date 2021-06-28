using UnityEngine;

public class HoverVisuals : Singleton<HoverVisuals>
{
    [SerializeField] GameObject startMarker;
    [SerializeField] GameObject endMarker;
    [SerializeField] LineRenderer lineRenderer;

    public Vector3 lineStartPos { get; set; }
    bool interacting = false;

    public void Interacting(bool isInteracting)
    {
        interacting = isInteracting;

        startMarker.SetActive(isInteracting);
        endMarker.SetActive(isInteracting);

        if(!isInteracting)
        {
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
        }
    }

    private void Update() 
    {
        if(interacting)
        {
            UpdateVisuals();
        }
    }

    public void UpdateVisuals()
    {
        if(lineStartPos != Vector3.zero)
        {
            RaycastHit raycastHit;

            Vector3 startPos = Vector3.zero;
            Vector3 endPos = Vector3.zero;

            bool hitGround = Physics.Raycast(lineStartPos, Vector3.down, out raycastHit, Mathf.Infinity, 1024);
            if(hitGround) //1024 == layer 10
            {
                endPos = raycastHit.point;
            } 
            else //We are hovering object outside of the level 
            {
                endPos = lineStartPos - new Vector3(0, 100, 0);
            }

            if(Physics.Raycast(endPos, Vector3.up, out raycastHit, Mathf.Infinity, 256))
            {
                startPos = raycastHit.point;
            } 
            else
            {
                startPos = lineStartPos;
            }

            startMarker.transform.position = startPos;
            endMarker.transform.position = endPos;

            lineRenderer.SetPositions(new Vector3[] { startPos, endPos });

            endMarker.SetActive(hitGround);
        }
    }
}