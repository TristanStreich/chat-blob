using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickandDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 previousMousePosition;
    private Rigidbody2D clickedRigidbody;
    private Rigidbody2D[] dynamicBodies;

    private void Start()
    {
        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("body");
        dynamicBodies = new Rigidbody2D[bodyObjects.Length];
        for (int i = 0; i < bodyObjects.Length; i++)
        {
            dynamicBodies[i] = bodyObjects[i].GetComponent<Rigidbody2D>();
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log("Clicked");
        isDragging = true;
        previousMousePosition = GetMouseWorldPosition();

        clickedRigidbody = GetComponent<Rigidbody2D>();
        clickedRigidbody.bodyType = RigidbodyType2D.Static;

        PetBehavior.PetBehav.canMove = false;

        MakeDynamicBodiesStayDynamic();
    }

    private void OnMouseUp()
    {
       // Debug.Log("letGo");
        isDragging = false;

        clickedRigidbody.bodyType = RigidbodyType2D.Dynamic;
        clickedRigidbody = null;

        PetBehavior.PetBehav.canMove = true;

        MakeDynamicBodiesStayDynamic();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2 currentMousePosition = GetMouseWorldPosition();
            Vector2 displacement = currentMousePosition - previousMousePosition;
            transform.position += (Vector3)displacement;
            previousMousePosition = currentMousePosition;
        }
    }

    private void MakeDynamicBodiesStayDynamic()
    {
        foreach (Rigidbody2D body in dynamicBodies)
        {
            if (body != clickedRigidbody)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mousePosition;
    }
}