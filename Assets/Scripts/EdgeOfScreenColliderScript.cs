using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeOfScreenColliderScript : MonoBehaviour
{
    public static EdgeOfScreenColliderScript _EdgeOfScreenColliderScript = null;

    private void Awake()
    {
        if (_EdgeOfScreenColliderScript == null)
        {
            _EdgeOfScreenColliderScript = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public float colDepth = 3f;
    public PhysicsMaterial2D friction;
    [HideInInspector]
    public Vector2 screenSize;
    private Transform topCollider;
    private Transform bottomCollider;
    private Transform rightCollider;
    private Transform leftCollider;
    public Vector3 CameraPosition;
    private float floorHeight = 0f; //0.48f out of editor;

    private void Start()
    {
#if !UNITY_EDITOR
        floorHeight = 0.48f;
#endif

        // Generate our empty objects
        topCollider = new GameObject().transform;
        bottomCollider = new GameObject().transform;
        rightCollider = new GameObject().transform;
        leftCollider = new GameObject().transform;


        // Name Our Objects
        topCollider.name = "TopCollider";
        bottomCollider.name = "BottomCollider";
        rightCollider.name = "RightCollider";
        leftCollider.name = "LeftCollider";

        //Layer Our Objects 
        int groundLayer = LayerMask.NameToLayer("Ground");
        int wallLayer = LayerMask.NameToLayer("Wall");
        leftCollider.gameObject.layer = wallLayer;
        rightCollider.gameObject.layer = wallLayer;
        bottomCollider.gameObject.layer = groundLayer;


        // Add Collider to Objects
        topCollider.gameObject.AddComponent<BoxCollider2D>().sharedMaterial = friction;
        bottomCollider.gameObject.AddComponent<BoxCollider2D>().sharedMaterial = friction;
        rightCollider.gameObject.AddComponent<BoxCollider2D>().sharedMaterial = friction;
        leftCollider.gameObject.AddComponent<BoxCollider2D>().sharedMaterial = friction;


        //Make them the child of Whatever Objects
        topCollider.parent = transform;
        bottomCollider.parent = transform;
        rightCollider.parent = transform;
        leftCollider.parent = transform;



        // Generate world space point Information
        CameraPosition = Camera.main.transform.position;
        screenSize.x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0f))) * 0.5f;
        screenSize.y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)), Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height))) * 0.5f;


        //Change our Scale and Position
        //RightCollider:
        rightCollider.localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
        rightCollider.position = new Vector3(CameraPosition.x + screenSize.x + (rightCollider.localScale.x * 0.5f), CameraPosition.y, 0f);
        //LeftCollider:
        leftCollider.localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
        leftCollider.position = new Vector3(CameraPosition.x - screenSize.x - (leftCollider.localScale.x * 0.5f), CameraPosition.y, 0f);
        //TopCollider:
        topCollider.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
        topCollider.position = new Vector3(CameraPosition.x, CameraPosition.y + screenSize.y + (topCollider.localScale.y * 0.5f), 0f);
        //BottomCollider:
        bottomCollider.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
        bottomCollider.position = new Vector3(CameraPosition.x, CameraPosition.y - screenSize.y - (bottomCollider.localScale.y * 0.5f) + floorHeight, 0f);
    }
}
