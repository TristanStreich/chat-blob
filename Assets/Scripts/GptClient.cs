using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using GptApi;


public static class GptClient
{
    private static string OPENAI_API_KEY = SecretsManager.GPTAPIKey;  //File.ReadAllText(".api_key");

    public static string systemPrompt;

    public static int MessageLimit;

    public static float temperature;

    /// <summary>
    /// Send a request to the gpt api with the list of messages.
    /// Then invoke the given callback with either a response or an error
    /// </summary>
    private static void sendRequest(List<Message> messages, ChatCallBack callback)
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

        /// make request
        UnityWebRequestAsyncOperation asyncRequest = www.SendWebRequest();

        /// register callback into async request event emitter
        asyncRequest.completed += (_) => {
            if (www.result == UnityWebRequest.Result.Success) {
                string jsonResponse = www.downloadHandler.text;
                ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(jsonResponse);

                callback.Invoke(chatResponse, null);
            }
            else {
                callback.Invoke(null, www.error);
            }
            www.Dispose();
        };
    }

    /// <summary>
    /// Send a message to Chat GPT. This will handle saving and sending chat context.
    /// To get results of this message listen for `GptEvent.ResponseReceived`
    /// </summary>
    public static void Chat(string message) {

        ChatLog.AddMessage(Message.User(message));
        List<Message> messages = ChatLog.GetLog(MessageLimit);
        GptEvent.Emitter.Invoke(new GptEvent.RequestSent());

        sendRequest(messages, (response, error) => {
            if (error != null) {
                GptEvent.Emitter.Invoke(new GptEvent.Error(error));
            } else {
                ChatLog.AddMessage(response.choices[0].message);
                GptEvent.Emitter.Invoke(new GptEvent.ResponseRecieved(response));
            }
        });
    }

    /// <summary>
    /// Function that will be called after the response from gpt is received.
    /// If there was an error while sending it will be in the string as the second arg
    /// </summary>
    public delegate void ChatCallBack(ChatResponse? response, String? error);
}