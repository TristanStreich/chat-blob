using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform targetTransform; // The target object whose position you want to copy
    private Rigidbody2D rb; // The Rigidbody2D component of the current object

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (targetTransform == null)
        {
            Debug.LogError("Target Transform not assigned! Please assign a target.");
        }
    }

    private void FixedUpdate()
    {
        if (targetTransform != null && rb != null)
        {
            // Copy the position of the target object
            rb.position = targetTransform.position;
        }
    }
}
