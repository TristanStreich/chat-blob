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

    [System.Serializable]
    private class ChatResponse
    {
        public string id;
        public string @object;
        public long created;
        public string model;
        public Usage usage;
        public ChatChoice[] choices;
    }

    [System.Serializable]
    private class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [System.Serializable]
    private class ChatChoice
    {
        public ChatMessage message;
        public string finish_reason;
        public int index;
    }

    [System.Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }


    public IEnumerator Chat(string inputText)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-3.5-turbo",
            messages = new Message[]
            { //TODO: will need to make this more complex to remember any context at all
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
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);

            if (chatResponse.choices != null && chatResponse.choices.Length > 0)
            {
                string messageContent = chatResponse.choices[0].message.content;
                // TODO: maybe send this message out of the coroutine with an event emitter?
                Debug.Log(messageContent);
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
        www.Dispose();
    }


    // Start is called before the first frame update
    void Start()
    {
        // put in chat input here for now.
        // TODO: maybe use event emitters to feed in input string as well
        StartCoroutine(Chat("Hi Blobby!!!"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}