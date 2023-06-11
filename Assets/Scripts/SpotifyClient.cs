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

        using (var httpClient = new HttpClient())
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
}