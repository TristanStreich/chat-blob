using SpotifyApi;
using UnityEngine;
using UnityEngine.Events;
using GptApi;

public abstract class GptEvent {

    public static UnityEvent<GptEvent> Emitter = new UnityEvent<GptEvent>();

    public class Error : GptEvent {
        public string message { get; }

        public Error(string message) {
            this.message = message;
        }
    }

    public class RequestSent : GptEvent {}

    public class ResponseRecieved : GptEvent
    {
        public ChatResponse response { get; }

        public ResponseRecieved(ChatResponse response) {
            this.response = response;
        }
    }

}