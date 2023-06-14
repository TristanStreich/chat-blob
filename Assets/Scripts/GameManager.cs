using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string bodyTag = "body";
    public float searchRadius = 1f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, searchRadius);

            if (colliders.Length > 0)
            {
                Collider2D nearestCollider = GetNearestCollider(colliders, mousePosition);
                if (nearestCollider.CompareTag(bodyTag))
                {
                    GrabObject(nearestCollider.gameObject, mousePosition);
                }
            }
        }
    }

    private Collider2D GetNearestCollider(Collider2D[] colliders, Vector2 referencePosition)
    {
        Collider2D nearestCollider = colliders[0];
        float nearestDistance = Vector2.Distance(referencePosition, nearestCollider.transform.position);

        for (int i = 1; i < colliders.Length; i++)
        {
            float distance = Vector2.Distance(referencePosition, colliders[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollider = colliders[i];
            }
        }

        return nearestCollider;
    }

    private void GrabObject(GameObject obj, Vector2 position)
    {
        // Snap the object to the mouse position
        obj.transform.position = position;

        // Perform any additional logic for grabbing the object
        ClickandDrag clickAndDrag = obj.GetComponent<ClickandDrag>();
        if (clickAndDrag != null)
        {
            //clickAndDrag.OnMouseDown();
            //clickAndDrag.OnMouseDrag(); // Trigger the OnMouseDrag event
        }
    }
}
