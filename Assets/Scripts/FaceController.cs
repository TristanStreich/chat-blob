using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.U2D;

public class FaceController : MonoBehaviour

    {
    public static FaceController FaceManager = null;

    public GameObject[] eyes;
    public GameObject Mouth;
    public float mouthOffset = 1f; // Distance between the lowest eye and the mouth

    private void Awake()
    {
        if (FaceManager == null)
        {
            FaceManager = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Update()
    {
        calcEyePos();
    }
    private void calcEyePos()
    {
        // Find the lowest eye position
        Vector3 lowestEyePosition = eyes[0].transform.position;
        for (int i = 1; i < eyes.Length; i++)
        {
            if (eyes[i].transform.position.y < lowestEyePosition.y)
            {
                lowestEyePosition = eyes[i].transform.position;
            }
        }

        // Calculate the target position below the lowest eye with the same x-coordinate as the midpoint between the eyes
        Vector3 targetPosition = lowestEyePosition;
        float midpointX = (eyes[0].transform.position.x + eyes[1].transform.position.x) * 0.5f;
        targetPosition.x = midpointX;
        targetPosition.y -= mouthOffset;

        // Move the mouth to the target position
        Mouth.transform.position = targetPosition;
    }
    public void ChangeFace(string Expression)
    {
        if (Expression.ToLower() == "happy")
        {
            for (int i = 0; i < eyes.Length; i++)
            {
                eyes[i].GetComponent<Animator>().Play("EyeHappy");
                Debug.Log("happy");
            }
        }

        if (Expression.ToLower() == "think")
        {
            for (int i = 0; i < eyes.Length; i++)
            {
                eyes[i].GetComponent<Animator>().Play("EyeThink");
                Debug.Log("thinking");
            }
        }
    }

   
}
