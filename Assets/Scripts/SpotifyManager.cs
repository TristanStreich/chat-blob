using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using SpotifyApi;

public class SpotifyManager : MonoBehaviour
{

    [Tooltip("How often we check spotify to see what is playing")]
    public float PollPeriodSeconds = 1f;

    private Track lastPlayed = null;
    private TrackAudioFeatures lastPlayedDetails = null;
    private bool isPlaying = false;
    

    void Start()
    {
        if (!SpotifyAuthClient.HasAccessToken()) {
            // we need to prompt user for login
            if (!File.Exists(SecretsManager.getPath(SecretsManager.Secret.spotify_client_secret))) {
                // we cannot init login until client secret
                // has been provided in secrets manager pop up
                return;
            }
            SpotifyAuthClient.initLogin();
        }

        // start event emitter system
        InvokeRepeating("UpdateCurrentlyPlaying", 0f, PollPeriodSeconds);
    }

    /// system which polls spotify to find if a song is playing.
    /// This will emit `StoppedPlaying` and `StartedPlaying` events on the 
    /// `SpotifyEvent.Emitter` subscribe to that to get events
    async void UpdateCurrentlyPlaying() {
        Track? currTrack = await SpotifyClient.CurrentlyPlaying();

        if (currTrack == null) {
            // null means nothing playing
            if (isPlaying) {
                isPlaying = false;
                SpotifyEvent.Emitter.Invoke(new SpotifyEvent.StoppedPlaying());
            }
            return;
        } else if (currTrack == lastPlayed) {
            // we are playing the same track as before
            if (!isPlaying) {
                // but we just unpaused
                isPlaying = true;
                SpotifyEvent.Emitter.Invoke(new SpotifyEvent.StartedPlaying(lastPlayed, lastPlayedDetails));
            }
            return;
        } else {
            // we are playing a song and we were not playing this song already
            // we first need to get the details from spotify
            TrackAudioFeatures currTrackDetails = await SpotifyClient.GetTrackDetails(currTrack);

            SpotifyEvent.Emitter.Invoke(new SpotifyEvent.StartedPlaying(currTrack, currTrackDetails));

            isPlaying = true;
            lastPlayed = currTrack;
            lastPlayedDetails = currTrackDetails;
        }
    }
}