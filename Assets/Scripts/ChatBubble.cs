using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    private SpriteRenderer WordBubble;
    private TextMeshPro textMeshPro;
    public Transform targetTransform;
    public float minDistance = 3f; // Minimum distance required to move the chat bubble
    public float movementSpeed = 5f;

    private void Awake()
    {
        WordBubble = transform.Find("Bkrnd").GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("BlobText").GetComponent<TextMeshPro>();
        SetUp("*blob sound*");
    }

    private void Start() {
        GptEvent.Emitter.AddListener(GptEventListener);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance between the chat bubble and the target transform
        float distance = Vector3.Distance(transform.position, targetTransform.position);

        // Check if the distance is greater than the minimum distance
        if (distance > minDistance)
        {
            // Move the chat bubble towards the target transform
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, Time.deltaTime * movementSpeed);
        }
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


    private void GptEventListener(GptEvent e) {
        switch (e) {
            case GptEvent.ResponseRecieved received:
                string message = received.response.choices[0].message.content;
                SetUp(message);
                break;
        }
    }
}
