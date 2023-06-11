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
    private static string OPENAI_API_KEY = File.ReadAllText(".api_key");

    public static string systemPrompt;

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
    /// Sends a chat request with a single message.
    /// </summary>
    /// <param name="singleMessage">The single message to send.</param>
    public static void Chat(string singleMessage) {
        Chat(new List<Message> { Message.User(singleMessage) }, defaultCallBack);
    }

    /// <summary>
    /// Sends a chat request with a list of messages.
    /// </summary>
    /// <param name="messages">The list of messages to send.</param>
    public static void Chat(List<Message> messages) {
        Chat(messages, defaultCallBack);
    }

    /// <summary>
    /// Sends a chat request with a single message and a callback function.
    /// Throws an error if the API response contains an error.
    /// </summary>
    /// <param name="singleMessage">The single message to send.</param>
    /// <param name="callback">The callback function to execute when the response is received.</param>
    public static void Chat(string singleMessage, ChatCallBackThrowOnError callback) {
        Chat(new List<Message> { Message.User(singleMessage) }, throwOnError(callback));
    }

    /// <summary>
    /// Sends a chat request with a list of messages and a callback function.
    /// Throws an error if the API response contains an error.
    /// </summary>
    /// <param name="messages">The list of messages to send.</param>
    /// <param name="callback">The callback function to execute when the response is received.</param>
    public static void Chat(List<Message> messages, ChatCallBackThrowOnError callback) {
        Chat(messages, throwOnError(callback));
    }

    /// <summary>
    /// Sends a chat request with a single message and a callback function.
    /// </summary>
    /// <param name="singleMessage">The single message to send.</param>
    /// <param name="callback">The callback function to execute when the response is received.</param>
    public static void Chat(string singleMessage, ChatCallBack callback) {
        Chat(new List<Message> { Message.User(singleMessage) }, callback);
    }

    /// <summary>
    /// Sends a chat request with a list of messages and a callback function.
    /// </summary>
    /// <param name="messages">The list of messages to send.</param>
    /// <param name="callback">The callback function to execute when the response is received.</param>
    public static void Chat(List<Message> messages, ChatCallBack callback) {
        sendRequest(messages, callback);
    }

    private static void defaultCallBack(ChatResponse? response, String? error) {
        if (error != null) {
            Debug.LogError("Error: " + error);
        } else if (response != null) {
            Debug.Log(response.choices[0].message.content);
        } else {
            throw new Exception("Bad invocation of gpt callback! Neither response nor error were provided");
        }
    }

    /// <summary>
    /// Converts a callback with no error input into a callback
    /// which raises an exception on error
    /// </summary>
    private static ChatCallBack throwOnError(ChatCallBackThrowOnError callback) {
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

    /// <summary>
    /// Function that will be called after the response from gpt is received.
    /// If there was an error while sending it will be in the string as the second arg
    /// </summary>
    public delegate void ChatCallBack(ChatResponse? response, String? error);

    /// <summary>
    /// Utility if you don't want to specify what to do with the error of the response.
    /// If you use this an expection will be raised if there was an error sending to gpt
    /// </summary>
    public delegate void ChatCallBackThrowOnError(ChatResponse response);
}