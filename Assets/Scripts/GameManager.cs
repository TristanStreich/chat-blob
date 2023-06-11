using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Application.OpenURL("http://localhost:3000/token/gen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}