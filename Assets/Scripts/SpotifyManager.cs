using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SpotifyManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(SpotifyClient.getIP());
        if (!SpotifyAuthClient.HasAccessToken()) {
            SpotifyAuthClient.startServer();
            Application.OpenURL(SpotifyAuthClient.login_url + "?" + QueryString());
        } else {
            Debug.Log("Token is Initialized!");
        }

        // CheckPlaying();

        InvokeRepeating("CheckPlaying", 0f, 1f);
    }

    string QueryString() {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("response_type", "code");
        queryString.Add("client_id", SpotifyAuthClient.client_id);
        queryString.Add("scope", "user-read-playback-state user-top-read user-read-recently-played");
        queryString.Add("redirect_uri", SpotifyAuthClient.redirect_uri);

        return queryString.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckPlaying() {
        SpotifyClient.IsPlayingMusic((is_playing) => {
            Debug.Log("Is playing? " + is_playing);
        });
    }
}