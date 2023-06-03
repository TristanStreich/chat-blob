using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour

    {
   
    public Transform[] eyes;
    public Transform Mouth;
    public float mouthOffset = 1f; // Distance between the lowest eye and the mouth

    private void Update()
    {
        // Find the lowest eye position
        Vector3 lowestEyePosition = eyes[0].position;
        for (int i = 1; i < eyes.Length; i++)
        {
            if (eyes[i].position.y < lowestEyePosition.y)
            {
                lowestEyePosition = eyes[i].position;
            }
        }

        // Calculate the target position below the lowest eye with the same x-coordinate as the midpoint between the eyes
        Vector3 targetPosition = lowestEyePosition;
        float midpointX = (eyes[0].position.x + eyes[1].position.x) * 0.5f;
        targetPosition.x = midpointX;
        targetPosition.y -= mouthOffset;

        // Move the mouth to the target position
        Mouth.transform.position = targetPosition;
    }
}
