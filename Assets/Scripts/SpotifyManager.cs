using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using SpotifyApi;

public class SpotifyManager : MonoBehaviour
{

    private Track lastPlayed = null;
    private TrackAudioFeatures lastPlayedDetails = null;
    private bool isPlaying = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!SpotifyAuthClient.HasAccessToken()) {
            if (!File.Exists(SecretsManager.getPath(SecretsManager.Secret.spotify_client_secret))) {
                // we cannot init login until client secret
                // has been provided in secrets manager pop up
                return;
            }
            // SpotifyAuthClient.startServer();
            // Application.OpenURL(SpotifyAuthClient.login_url + "?" + QueryString());
            SpotifyAuthClient.initLogin();
        } else {
            Debug.Log("Token is Initialized!"); //TODO: remove
        }

        InvokeRepeating("UpdateCurrentlyPlaying", 0f, 1f);
    }

    // string QueryString() {
    //     NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
    //     queryString.Add("response_type", "code");
    //     queryString.Add("client_id", SpotifyAuthClient.client_id);
    //     queryString.Add("scope", "user-read-playback-state user-top-read user-read-recently-played");
    //     queryString.Add("redirect_uri", SpotifyAuthClient.redirect_uri);

    //     return queryString.ToString();
    // }


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

            isPlaying = true;
            SpotifyEvent.Emitter.Invoke(new SpotifyEvent.StartedPlaying(currTrack, currTrackDetails));

            lastPlayed = currTrack;
            lastPlayedDetails = currTrackDetails;
        }
    }
}