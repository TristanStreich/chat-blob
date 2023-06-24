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
        LayoutGroup = FindObjectOfType<VerticalLayoutGroup>();
    }

    public void SendMessage()
    {
        GptClient.Chat(TextInput.text);
        TextInput.text = "";
    }

    public void RefreshTextLayout() //we need this because the autosizer for text boxes is busted and needs to be reminded that it can change sizes
    {
        LayoutGroup.enabled = false;
        LayoutGroup.enabled = true;
    }
    private void Update()
    {
        
            if (Input.GetKeyUp(KeyCode.Return)) { SendMessage(); }
        
    }
}