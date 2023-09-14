using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiSquish : MonoBehaviour
{
    private Camera mainCamera;
    private BlobClickandDrag[] DragScripts;
    public CircleCollider2D SquishTrigger;
    public LayerMask antiSquish;

    private void Awake()
    {
        DragScripts = FindObjectsOfType<BlobClickandDrag>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Cast a ray from the mouse position
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, antiSquish);

        if (hit.collider != null)
        {
            // Check if the raycast hits an object with the "Blob" tag
            if (hit.collider == SquishTrigger)
            {
                
                foreach (BlobClickandDrag script in DragScripts)
                {
                    if (script.isDragging)
                    {
                        Debug.Log("Anti Squish Defense System");
                        script.TooFast();
                    }  
                }
            }
        }
    }

}
