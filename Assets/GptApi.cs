using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class GptApi : MonoBehaviour
{

    private const string OPENAI_API_KEY = "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";
    private const string SYSTEM_PROMPT = "You are a little helper named Bloby, who sits in my windows 11 desktop and answers questions about my file system in a cheerful way";

    [System.Serializable]
    private class ChatRequest
    {
        public string model;
        public Message[] messages;
        public float temperature;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    private class ChatResponse
    {
        public ChatChoice[] choices;
    }

    private class ChatChoice
    {
        public ChatMessage message;
    }

    private class ChatMessage
    {
        public string content;
    }

    public IEnumerator Chat(string inputText)
    {
        Debug.Log("Starting Coroutine");
        string url = "https://api.openai.com/v1/chat/completions";

        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-3.5-turbo",
            messages = new Message[]
            {
                new Message { role = "system", content = SYSTEM_PROMPT },
                new Message { role = "user", content = inputText }
            },
            temperature = 0.7f
        };

        string jsonBody = JsonUtility.ToJson(chatRequest);
        byte[] postData = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest www = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(postData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + OPENAI_API_KEY);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log(jsonResponse);
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);

            if (chatResponse.choices != null && chatResponse.choices.Length > 0)
            {
                string messageContent = chatResponse.choices[0].message.content;
                Debug.Log(messageContent);
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        Debug.Log("Finished Coroutine");
        www.Dispose();
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Script");
        StartCoroutine(Chat("Hi Blobby!!!"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}