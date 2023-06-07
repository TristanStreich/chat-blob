using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BlobSpriteRender : MonoBehaviour
{
    #region Constants
    private const float splineOffset = 0.5f;
    #endregion
    #region Fields
    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public Transform[] points;

    private SpriteShapeRenderer spriteRenderer;

    [Header ("Party Time")]
    public bool PartyLikeNobodyIsWatching;
    public float DanceAmount = 0.5f; // Adjust the maximum offset threshold
    private float[] vertexOffsets;
    public float DanceSpeed;
    private float timer;
    private float hue;
    public float ColortransitionSpeed = 1f; // Adjust the speed of the color transition
    #endregion


    private void Awake()
    {
        vertexOffsets = new float[points.Length];
        timer = DanceSpeed;
        spriteRenderer = FindObjectOfType<SpriteShapeRenderer>();
        hue = 0f; // Initial hue value (red)
        UpdateVerticies();
    }

    private void Update()
    {
        
            PartyMode(PartyLikeNobodyIsWatching);

            UpdateVerticies();
    }

    private void PartyMode(bool PartyTime)
    {
        if (PartyTime)
        {
            goRainbow();
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                for (int i = 0; i < points.Length; i++) //randomly set the offset to the dance amount
                {
                    // Update the offset for the current vertex
                    vertexOffsets[i] = Random.Range(-DanceAmount, DanceAmount);
                }
                timer = DanceSpeed;
            }
        }    
        else
        {
            spriteRenderer.color = Color.white;
            for (int i = 0; i < points.Length; i++) //randomly set the offset to the dance amount
            {
                // Update the offset for the current vertex
                vertexOffsets[i] = 0;
            }
        }
    }
    public void goRainbow()
    {
        // Increase the hue value over time
        hue += ColortransitionSpeed * Time.deltaTime;

        // Wrap the hue value to stay within the range of 0 to 1
        if (hue > 1f)
            hue -= 1f;

        // Convert the hue value to RGB color
        Color newColor = Color.HSVToRGB(hue, 1f, 1f);

        // Assign the new color to the Sprite Shape Renderer
        spriteRenderer.color = newColor;
    }

    private void UpdateVerticies()

    {

        for (int i = 0; i < points.Length - 1; i++)
        {


            Vector2 _vertex = points[i].localPosition;
            Vector2 _towardsCenter = (Vector2.zero - _vertex).normalized;

            float _colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            try
            {
                spriteShape.spline.SetPosition(i, (_vertex - _towardsCenter *  (_colliderRadius + vertexOffsets[i])));
            }

            catch
            {
                //Debug.Log("Spline points are too close to each other.. recalculate");
                spriteShape.spline.SetPosition(i, (_vertex - _towardsCenter * (_colliderRadius + splineOffset + vertexOffsets[i])));
            }

            Vector2 _lt = spriteShape.spline.GetLeftTangent(i);

            Vector2 _newLt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            Vector2 _newRt = Vector2.zero - (_newLt);

            spriteShape.spline.SetRightTangent(i, _newRt);
            spriteShape.spline.SetLeftTangent(i, _newLt);

        }
    }

}


