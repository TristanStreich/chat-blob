using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using UnityEngine.Networking;


public class GptApi : MonoBehaviour
{
    //to make it a singleton
    public static GptApi gpt = null;

    private string OPENAI_API_KEY = File.ReadAllText(".api_key");

    [TextArea(15,20)]
    public string systemPrompt = "You are a little helper named Bloby, who sits in my windows 11 desktop and answers questions about my file system in a cheerful way";

    [Range(0F, 1F)]
    public float temperature = 0.7F;

    [System.Serializable]
    private class ChatRequest
    {
        public string model;
        public List<Message> messages;
        public float temperature;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;

        public static Message User(string content) {
            return new Message {
                role = "user",
                content = content
            };
        }

        public static Message System(string content) {
            return new Message {
                role = "system",
                content = content
            };
        }

        public static Message Assistant(string content) {
            return new Message {
                role = "assistant",
                content = content
            };
        }
    }

    [System.Serializable]
    public class ChatResponse
    {
        public string id;
        public string @object;
        public long created;
        public string model;
        public Usage usage;
        public Choice[] choices;
    }

    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
        public string finish_reason;
        public int index;
    }

    private void Awake()
    {
        if (gpt == null)
        {
            gpt = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    /// TODO: doc comments
    private IEnumerator chat(List<Message> messages, ChatCallBack callback)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        messages.Insert(0, Message.System(systemPrompt));

        ChatRequest chatRequest = new ChatRequest {
            model = "gpt-3.5-turbo",
            messages = messages,
            temperature = temperature
        };

        string jsonBody = JsonUtility.ToJson(chatRequest);
        byte[] postData = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest www = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(postData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + OPENAI_API_KEY);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            string jsonResponse = www.downloadHandler.text;
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);

            callback.Invoke(chatResponse, null);
        }
        else {
            callback.Invoke(null, www.error);
        }
        www.Dispose();
    }



    public void Chat(string singleMessage) {
        Chat(new List<Message> { Message.User(singleMessage) }, defaultCallBack);
    }

    public void Chat(List<Message> messages) {
        Chat(messages, defaultCallBack);
    }
    
    public void Chat(string singleMessage, ChatCallBackThrowOnError callback) {
        Chat(new List<Message> { Message.User(singleMessage) }, throwOnError(callback));
    }


    public void Chat(List<Message> messages, ChatCallBackThrowOnError callback) {
        Chat(messages, throwOnError(callback));
    }

    public void Chat(string singleMessage, ChatCallBack callback) {
        Chat(new List<Message> { Message.User(singleMessage) }, callback);
    }


    public void Chat(List<Message> messages, ChatCallBack callback) {
        StartCoroutine(chat(messages, callback));
    }

    private void defaultCallBack(ChatResponse? response, String? error) {
        if (error != null) {
            Debug.LogError("Error: " + error);
        } else if (response != null) {
            Debug.Log(response.choices[0].message.content);
        } else {
            throw new Exception("Bad invocation of gpt callback! Neither response nor error were provided");
        }
    }

    private ChatCallBack throwOnError(ChatCallBackThrowOnError callback) {
        return (response, error) => {
            if (error != null) {
                throw new Exception("Unhandled error while making call to GPT:" + error);
            } else if (response != null) {
                callback.Invoke(response);
            } else {
                throw new Exception("Bad invocation of gpt callback! Neither response nor error were provided");
            }
        };
    }

    /// TODO:doc comments
    public delegate void ChatCallBack(ChatResponse? response, String? error);
    public delegate void ChatCallBackThrowOnError(ChatResponse response);

    // Start is called before the first frame update
    void Start()
    {
        Chat("Hi Blobby!!!");
    }
}