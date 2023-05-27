using UnityEngine;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class ChatCoroutine : MonoBehaviour
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

        string jsonBody = JsonConvert.SerializeObject(chatRequest);
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
            Dictionary<string, object> chatResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

            List<Dictionary<string, object>> choices = chatResponse["choices"] as List<Dictionary<string, object>>;
            if (choices != null && choices.Count > 0)
            {
                Dictionary<string, object> message = choices[0]["message"] as Dictionary<string, object>;
                if (message != null && message.ContainsKey("content"))
                {
                    string messageContent = message["content"].ToString();
                    Debug.Log(messageContent);
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        www.Dispose();
    }
    }
}
