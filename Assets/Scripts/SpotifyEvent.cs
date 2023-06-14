using SpotifyApi;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpotifyEvent {

    public static UnityEvent<SpotifyEvent> Emitter = new UnityEvent<SpotifyEvent>();

    public class StoppedPlaying : SpotifyEvent {}

    public class StartedPlaying : SpotifyEvent
    {
        public Track track { get; }
        public TrackAudioFeatures details { get; }

        public StartedPlaying(Track track, TrackAudioFeatures details)
        {
            track = track;
            details = details;
        }
    }

}