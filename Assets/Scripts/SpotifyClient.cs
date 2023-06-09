using UnityEngine.Networking;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi;

public static class SpotifyClient {

    private static async Task<string> MakeRequestWithRefresh(string url) {

        HttpResponseMessage response = await MakeSingleRequest(url);

        if (response.StatusCode == HttpStatusCode.Unauthorized) {
            Debug.Log("Got 401. Reauthenticating");
            await SpotifyAuthClient.RefreshToken();
            response = await MakeSingleRequest(url);
        }

        string response_body = await response.Content.ReadAsStringAsync();


        if (!response.IsSuccessStatusCode) {
            Debug.LogError(response_body);
            throw new Exception(response_body);
        }

        return response_body;

    }

    private static async Task<HttpResponseMessage> MakeSingleRequest(string url) {
        string access_token = SpotifyAuthClient.getAccessTokenFromFile();

        using (var httpClient = new HttpClient()) // TODO probaly dont create a new client with each request :facepalm:
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + access_token);
            return await httpClient.GetAsync(url);
        }
    }

    /// public api
    public static async Task MakeRequest(string url, Action<string> callback) {
        var output = await MakeRequestWithRefresh(url);
        callback.Invoke(output);
    }


    public static async Task IsPlayingMusic(Action<bool> callback) {
        var output = await MakeRequestWithRefresh("https://api.spotify.com/v1/me/player");

        try {
            PlaybackResponse response = JsonUtility.FromJson<PlaybackResponse>(output);
            callback.Invoke(response.is_playing);
        } catch {
            callback.Invoke(false);
        }
    }

    public static async Task<Track?> CurrentlyPlaying() {
        string output = await MakeRequestWithRefresh("https://api.spotify.com/v1/me/player");

        // Debug.Log(output);

        PlaybackResponse response;
        try {
            response = JsonUtility.FromJson<PlaybackResponse>(output);
            if (response == null) {
                return null;
            }
        } catch {
            return null;
        }

        if (response.currently_playing_type == "unknown") {
            Debug.LogWarning($"Found unknown playing type. Repolling. {output}");
            return await CurrentlyPlaying();
        }

        if (response.is_playing) {
            return response.item;
        } else {
            return null;
        }
    }

    public static async Task<TrackAudioFeatures?> GetTrackDetails(Track track) {
        string output = await MakeRequestWithRefresh("https://api.spotify.com/v1/audio-features/" + track.id);
        return JsonUtility.FromJson<TrackAudioFeatures>(output);
    }
}