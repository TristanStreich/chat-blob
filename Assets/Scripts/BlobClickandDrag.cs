using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlobClickandDrag : MonoBehaviour
{
    [HideInInspector]
    public bool isDragging = false;

    private Vector2 previousMousePosition;
    
    [HideInInspector]
    public Rigidbody2D[] dynamicBodies;
    private Rigidbody2D clickedRigidbody;
    //private Rigidbody2D Blob;
    private float mouseSpeedThreshold =55f;
    private float bodySpeedThreshold = 25f;
    private float distanceThreshold = 20f; // Distance threshold from the screen edges
    private float bottomDistanceThreshold = 10f; // Distance threshold from the screen bottom specififcally

    public Rigidbody2D MainBody;
    public GameObject ParticleBlob;


    private void Start()
    {
        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("Body");
        dynamicBodies = new Rigidbody2D[bodyObjects.Length];
        for (int i = 0; i < bodyObjects.Length; i++)
        {
            dynamicBodies[i] = bodyObjects[i].GetComponent<Rigidbody2D>();
        }
    }

    Vector3 spawnPosition = Vector3.zero;
    Vector3 mousePosition = Vector3.zero;
    private void Update()
    {
        if(isDragging)
        {
            
            // Get the mouse position in screen coordinates
            mousePosition = Input.mousePosition;

            // Check if the mouse position is within the distance threshold from the screen edges
            if (mousePosition.x <= distanceThreshold ||
                mousePosition.x >= Screen.width - distanceThreshold ||
                mousePosition.y <= bottomDistanceThreshold ||
                mousePosition.y >= Screen.height - distanceThreshold)
            {
                TooFast();
                Debug.Log("I cant let you drag past the edges!");
            }

            if (MainBody.velocity.magnitude >= bodySpeedThreshold)
            {
                TooFast();
                Debug.Log("The boy escaped");
            }
        }
        
    }

    private void OnMouseDown()
    {
        //Debug.Log("Clicked");
        isDragging = true;
        
        previousMousePosition = GetMouseWorldPosition();

        clickedRigidbody = GetComponentInParent<Rigidbody2D>();
        clickedRigidbody.bodyType = RigidbodyType2D.Static;
        clickedRigidbody.position = GetMouseWorldPosition();
        
        Cursor.visible = false;

        PetBehavior.PetBehav.isHeld = true;

        MakeDynamicBodiesStayDynamic();
    }

    private void OnMouseUp()
    {
        //Debug.Log("letGo");
        if (isDragging)
        {
            clickedRigidbody.bodyType = RigidbodyType2D.Dynamic;
            clickedRigidbody = null;

            isDragging = false;

            Cursor.visible = true; //this is because the mouse is faster than the updated following render

            PetBehavior.PetBehav.isHeld = false;

            MakeDynamicBodiesStayDynamic();
        } 
    }



    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2 currentMousePosition = GetMouseWorldPosition();
            Vector2 mouseDelta = currentMousePosition - previousMousePosition;
            previousMousePosition = currentMousePosition;

            clickedRigidbody.position = GetMouseWorldPosition();

            float currentSpeed = mouseDelta.magnitude / Time.deltaTime;
            if (currentSpeed >= mouseSpeedThreshold)
            {
                TooFast();
                Debug.Log("Mouse too Fast!");
            }
        }
    }
    public void TooFast()
    {
        //Debug.Log("Escaped!");
        // Add particles and extra sounds when we get to it
        clickedRigidbody.bodyType = RigidbodyType2D.Dynamic;
        clickedRigidbody = null;

        isDragging = false;

        Cursor.visible = true; //this is because the mouse is faster than the updated following render

        PetBehavior.PetBehav.isHeld = false;

        // Convert mouse position to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate the direction from the object to the mouse
        Vector3 direction = worldPosition - transform.position;

        // Calculate the rotation to face the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        spawnPosition = Vector3.Lerp(gameObject.transform.position, worldPosition, 0.5f);
        GameObject particle = Instantiate(ParticleBlob, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.AngleAxis(angle, Vector3.forward));

        MakeDynamicBodiesStayDynamic();
        
    }

    private void MakeDynamicBodiesStayDynamic()
    {
        foreach (Rigidbody2D body in dynamicBodies)
        {
            if (body != clickedRigidbody)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
            else return;
        }

    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mousePosition;
    }
}