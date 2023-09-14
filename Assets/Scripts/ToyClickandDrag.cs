using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyClickandDrag : MonoBehaviour
{

    private Vector2 previousMousePosition;
    private float currentSpeed;
    bool clicked = false;
    public float maxSpeed = 5.0f;
    public float minSpeed = 1.0f;
    public float distanceThreshold = 2.0f;

    private Rigidbody2D rb; // Use Rigidbody for 2D, Rigidbody for 3D

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Use GetComonent<Rigidbody>() for 3D
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, mousePosition);

        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, distance / distanceThreshold);
        Vector3 targetVelocity = direction * targetSpeed;
        if (clicked)
        {
            rb.velocity = targetVelocity;
        }
        
    }

    private void OnMouseDown()
    {
        Cursor.visible = false;
        clicked = true;
    }

    private void OnMouseUp()
    {     
        Cursor.visible = true; //this is because the mouse is faster than the updated following render
        clicked = false;
    }

   

   
}
