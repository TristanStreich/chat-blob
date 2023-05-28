using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        StartCoroutine(GptApi.gpt.Chat(TextInput.text));
        TextInput.text = "";
    }
   
}
