using System.Collections.Generic;

/// Defines the request and response of the gpt api
namespace GptApi {
    
    [System.Serializable]
    public class ChatRequest
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
}