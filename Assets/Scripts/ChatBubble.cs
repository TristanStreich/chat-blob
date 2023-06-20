using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private SpriteRenderer WordBubble;
    private TextMeshPro textMeshPro;
    private void Awake()
    {
        WordBubble = transform.Find("Bkrnd").GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("BlobText").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        SetUp("*blob sound*");
    }

    private void SetUp(string input)
    {
        textMeshPro.SetText(input);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(3f, 1f);
        WordBubble.size = textSize + padding;

        Vector3 offset = new Vector3(0, 0); //Only change offset if we want extra elements to share the space of the word bubble
        //WordBubble.transform.localPosition = new Vector3(WordBubble.size.x/2f , 0f) + offset;
    }
}
