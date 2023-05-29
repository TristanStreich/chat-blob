using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static GptApi.ChatResponse;

public class InputAndDisplay : MonoBehaviour
{
    public static InputAndDisplay UIInput = null;

    public TMP_Text TextInput;
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

    public void SendMessage()
    {
        GptApi.gpt.Chat(TextInput.text, HandleGPTResponse);
        TextInput.text = "";
    }


    private void HandleGPTResponse(GptApi.ChatResponse? response, string? error) {
        var x = response.choices[0];
        if (error != null) {
            Debug.LogError("Error: " + error);
            GPTTextDisplay.text = ("Error: " + error);
        } else if (response != null) {
                string messageContent = response.choices[0].message.content;
                Debug.Log(messageContent);
                GPTTextDisplay.text = messageContent;
        } else {
            throw new Exception("Bad GPT callback invocation. No response or error provided");
        }
    }
}
