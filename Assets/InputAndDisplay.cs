using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GptApi;

public class InputAndDisplay : MonoBehaviour
{
    public static InputAndDisplay UIInput = null;

    // these two vars probably belong in gpt manager
    public int MessageLimit = 10;
    public string startingMessage = "Hi Blobby!";

    public TMP_InputField TextInput;
    public TMP_Text GPTTextDisplay;

    private void Awake()
    {
        if (UIInput == null)
        {
            UIInput = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void Start() {
        ChatLog.AddMessage(startingMessage);
        GptClient.Chat(startingMessage, HandleGPTResponse);
    }

    public void SendMessage()
    {
        ChatLog.AddMessage(TextInput.text);
        List<Message> context = ChatLog.GetLog(MessageLimit);
        GptClient.Chat(context, HandleGPTResponse);
        TextInput.text = "";
    }


    private void HandleGPTResponse(ChatResponse? response, string? error) {
        if (error != null) {
            Debug.LogError("Error: " + error);
            GPTTextDisplay.text = ("Error: " + error);
        } else if (response != null) {
                Message responseMessage = response.choices[0].message;
                Debug.Log(responseMessage.content);
                GPTTextDisplay.text = responseMessage.content;
                ChatLog.AddMessage(responseMessage);
        } else {
            throw new Exception("Bad GPT callback invocation. No response or error provided");
        }
    }
}
