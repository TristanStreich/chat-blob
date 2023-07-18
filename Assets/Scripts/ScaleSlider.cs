using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSlider : MonoBehaviour
{
    [Range(2, 5)]
    public int floatRange;


    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = new Vector3 (floatRange * 0.1f, floatRange * 0.1f, 1);
        gameObject.transform.localScale = newScale;
    }
}
