using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSlider : MonoBehaviour
{
    [Range(0.2f, 2f)]
    public float floatRange;


    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = new Vector3 (floatRange, floatRange, 1);
        gameObject.transform.localScale = newScale;
    }
}
