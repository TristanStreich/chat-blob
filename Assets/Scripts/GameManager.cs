using System;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager GameMan = null;

    public Transform SpawnPos;

    public Transform PlayerPos;

    [Header("Spawnables")]
    public GameObject GrowFood;
    public GameObject ShrinkFood;
    public GameObject Ball;

    private float minXOffset = -2.0f; // Minimum X offset
    private float maxXOffset = 2.0f; // Maximum X offset
    private float minYOffset = -2.0f; // Minimum Y offset
    private float maxYOffset = 2.0f; // Maximum Y offset

    private GameObject MicroOptions;
    private RectTransform MicroRec;
    public RectTransform CanvasRec;
    public Camera mainCamera;

    private void Awake()
    {
        if (GameMan == null)
        {
            GameMan = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    private void Start()
    {
        //setting up the ability to Right Click Blob
        MicroOptions = GameObject.Find("MicroOptions");
        MicroRec = MicroOptions.GetComponent<RectTransform>();
        if (MicroOptions == null)
        {
            Debug.Log("The right click actions are name dependent");
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            OpenMicroMenu();
        } 
    }
    
    public void OpenMicroMenu()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 between = mousePositionWorld - new Vector2(PlayerPos.position.x, PlayerPos.position.y);
        if (between.magnitude < 2)
        {
            MicroOptions.SetActive(true);
            MicroRec.position = new Vector3(mousePosition.x, mousePosition.y, CanvasRec.position.z);
        }
        else { MicroOptions.SetActive(false); }
    }

    private void SpawnObject(GameObject spawned)
    {
        float randomXOffset = UnityEngine.Random.Range(minXOffset, maxXOffset);
        float randomYOffset = UnityEngine.Random.Range(minYOffset, maxYOffset);

        Vector3 spawnPosition = SpawnPos.position + new Vector3(randomXOffset, randomYOffset, 0);

        if (PlayerPos.position.x < 0)
        {
            spawnPosition.x *= -1;
        }
      
        Instantiate(spawned, spawnPosition, Quaternion.identity);
    }

    public void _GrowFoodSpawn()
    {
        SpawnObject(GrowFood);
    }
    public void _ShrinkFoodSpawn()
    {
        SpawnObject(ShrinkFood);
    }
    public void _BallSpawn()
    {
        SpawnObject(Ball);
    }

}
