using System.Collections.Generic;
using GptApi;

public static class ChatLog {
    private static List<Message> log;

    static ChatLog()
    {
        ChatLog.log = new List<Message>();
    }

    public static void AddMessage(Message message) {
        ChatLog.log.Add(message);
    }

    /// <summary>
    /// Assumes the input string is a user message and appends to log
    /// </summary>

    public static void AddMessage(string message) {
        ChatLog.log.Add(Message.User(message));
    }

    /// <summary>
    /// Returns entire log
    /// </summary>
    public static List<Message> GetLog() {
        return ChatLog.log;
    }

    /// <summary>
    /// Retrieves a list of messages from the log, containing the last <paramref name="messageWindow"/> messages.
    /// </summary>
    /// <param name="messageWindow">The maximum number of messages to retrieve from the end of the log.</param>
    /// <returns>A list of messages from the log.</returns>
    public static List<Message> GetLog(int messageWindow) {
        if (ChatLog.log.Count <= messageWindow) {
            return ChatLog.log;
        } else {
            return ChatLog.log.GetRange(ChatLog.log.Count - messageWindow, messageWindow);
        }
    }
}