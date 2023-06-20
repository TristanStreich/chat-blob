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

    [Header("Visible Text and Input")]
    public TMP_InputField TextInput;
    public TMP_Text GPTTextDisplay;
    
    private VerticalLayoutGroup LayoutGroup;

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
        GptEvent.Emitter.AddListener(GptEventListener);

        LayoutGroup = FindObjectOfType<VerticalLayoutGroup>();
        // GptClient.newChat(startingMessage); //, HandleGPTResponse);
    }

    public void SendMessage()
    {
        GptClient.newChat(TextInput.text);
        TextInput.text = "";
        // FaceController.FaceManager.ChangeFace("think");
        // PetBehavior.PetBehav.canMove = false; //make blob sit still to think
    }

    public void RefreshTextLayout() //we need this because the autosizer for text boxes is busted and needs to be reminded that it can change sizes
    {
        LayoutGroup.enabled = false;
        LayoutGroup.enabled = true;
    }
    // private void HandleGPTResponse(ChatResponse? response, string? error) {
    //     if (error != null) {
    //         Debug.LogError("Error: " + error);
    //         GPTTextDisplay.text = ("Error: " + error);
    //     } else if (response != null) {
    //             Message responseMessage = response.choices[0].message;
    //             Debug.Log(responseMessage.content);
    //             GPTTextDisplay.text = responseMessage.content;
    //             ChatLog.AddMessage(responseMessage);
    //     } else {
    //         throw new Exception("Bad GPT callback invocation. No response or error provided");
    //     }
    // }
    private void Update()
    {
        
            if (Input.GetKeyUp(KeyCode.Return)) { SendMessage(); }
        
    }


    private void GptEventListener(GptEvent e) {
        switch (e) {
            case GptEvent.ResponseRecieved received:
                ChatResponse response = received.response;
                Message responseMessage = response.choices[0].message;
                // Debug.Log(responseMessage.content);
                GPTTextDisplay.text = responseMessage.content;
                ChatLog.AddMessage(responseMessage);
                break;
        }
    }
}