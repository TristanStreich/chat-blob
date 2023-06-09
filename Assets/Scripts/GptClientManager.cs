using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Specifically for interacting with the client through unity gui in developer.
/// Should probably remove before release
public class GptClientManager : MonoBehaviour
{


    [Range(0F, 1F)]
    public float temperature = 0.7F;

    public int MessageLimit = 10;

    [TextArea(3,4)]
    public string StartingMessage = "Hi Blobby!";

    [TextArea(15,20)]
    public string systemPrompt = "You are a little helper named Bloby, who sits in my windows 11 desktop and answers questions about my file system in a cheerful way";

    private float prevTemp;

    private string prevSystemPrompt;

    private int prevLimit;

    // Start is called before the first frame update
    void Start()
    {
        GptEvent.Emitter.AddListener(ErrorLogger);

        prevTemp = temperature;
        prevSystemPrompt = systemPrompt;
        prevLimit = MessageLimit;
        GptClient.systemPrompt = systemPrompt;
        GptClient.temperature = temperature;
        GptClient.MessageLimit = MessageLimit;

        GptClient.Chat(StartingMessage);
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
        if (MessageLimit != prevLimit) {
            prevLimit = MessageLimit;
            GptClient.MessageLimit = MessageLimit;
        }
    }


    // Logs Error responses from gpt client to console.
    //
    // There is nothing else listening for errors atm so
    // be careful removing this as no listeners means it
    // will be hard to realize there is an error happening
    // with gpt.
    // 
    // eventually we can build in error listeners for things
    // like face changer and maybe give him X eyes or something
    private void ErrorLogger(GptEvent e) {
        switch (e) {
            case GptEvent.Error error:
                Debug.LogError(error.message);
                break;
        }
    }
}
