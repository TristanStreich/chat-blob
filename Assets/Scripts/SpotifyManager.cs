using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;

public class SpotifyManager : MonoBehaviour
{

    public BlobSpriteRender spriteRender;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!SpotifyAuthClient.HasAccessToken()) {
            if (!File.Exists(SecretsManager.getPath(SecretsManager.Secret.spotify_client_secret))) {
                return;
            }
            SpotifyAuthClient.startServer();
            Application.OpenURL(SpotifyAuthClient.login_url + "?" + QueryString());
        } else {
            Debug.Log("Token is Initialized!");
        }


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
            spriteRender.PartyLikeNobodyIsWatching = is_playing;
        });
    }
}