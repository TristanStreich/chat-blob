using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Specifically for interacting with the client through unity gui in developer.
/// Should probably remove before release
public class GptClientManager : MonoBehaviour
{

    [TextArea(15,20)]
    public string systemPrompt = "You are a little helper named Bloby, who sits in my windows 11 desktop and answers questions about my file system in a cheerful way";

    [Range(0F, 1F)]
    public float temperature = 0.7F;


    private float prevTemp;

    private string prevSystemPrompt;

    // Start is called before the first frame update
    void Start()
    {
        prevTemp = temperature;
        prevSystemPrompt = systemPrompt;
        GptClient.systemPrompt = systemPrompt;
        GptClient.temperature = temperature;
    }

    // Update is called once per frame
    void Update()
    {
        if (temperature != prevTemp) {
            prevTemp = temperature;
            GptClient.temperature = temperature;
        }
        if (systemPrompt != prevSystemPrompt) {
            prevSystemPrompt = systemPrompt;
            GptClient.systemPrompt = systemPrompt;
        }
    }
}
