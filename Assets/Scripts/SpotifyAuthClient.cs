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

public static class SpotifyAuthClient {

    public static string client_secret = SecretsManager.SpotifyClientSecret;
    public static string client_id = "e6caa7be19934d19bc01701139e19df7";
    public static string client_auth = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(client_id + ":" + client_secret));

    private static uint port = getPort();

    private static SimpleHTTPServer server;

    public static string redirect_uri = String.Format("http://localhost:{0}/blobby/return", port);

    // hit this to go through login portal
    public static string login_url = "https://accounts.spotify.com/authorize";

    // hit this with code from login portal to get access token
    private static string token_url = "https://accounts.spotify.com/api/token";

    /// TODO: find open port
    private static uint getPort() {
        return 4321;
    }


    public static void startServer() {
        server = new SimpleHTTPServer("http://localhost:" + port + "/");
    }

    private static void stopServer() {
        server.Stop();
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string access_token;
        public string token_type;
        public string scope;
        public int expires_in;
        public string refresh_token;
    }



    public static async Task redirectRoute(HttpListenerRequest request) {
        string code = request.QueryString["code"];
        TokenResponse response = await getAccessToken(code, null);

        SaveToken(response);
        stopServer();
    }

    public static void SaveToken(TokenResponse response) {
        Directory.CreateDirectory(SecretsManager.SecretsDir);

        if (!string.IsNullOrEmpty(response.access_token)) {
            string path = SecretsManager.SecretsDir + "/access_token";
            File.WriteAllText(path, response.access_token);
        }
        if (!string.IsNullOrEmpty(response.refresh_token)) {
            string path = SecretsManager.SecretsDir + "/refresh_token";
            File.WriteAllText(path, response.refresh_token);
        }
    }

    /// Use refresh token to get new access token from spotify and save it
    public static async Task RefreshToken() {
        string refresh_token = getRefreshTokenFromFile();
        TokenResponse response = await getAccessToken(null, refresh_token);
        SaveToken(response);
    }

    // If this is false then we need to open the login window
    public static bool HasAccessToken() {
        return File.Exists(SecretsManager.SecretsDir + "/access_token");
    }

    public static string getAccessTokenFromFile() {
        return File.ReadAllText(SecretsManager.SecretsDir + "/access_token");
    }

    public static string getRefreshTokenFromFile() {
        return File.ReadAllText(SecretsManager.SecretsDir + "/refresh_token");
    }

    public static async Task<TokenResponse> getAccessToken(string code, string refreshToken)
    {
        var form = new Dictionary<string, string>();
        
        if (!string.IsNullOrEmpty(code))
        {
            form["code"] = code;
            form["grant_type"] = "authorization_code";
            form["redirect_uri"] = redirect_uri;
        }
        else if (!string.IsNullOrEmpty(refreshToken))
        {
            form["grant_type"] = "refresh_token";
            form["refresh_token"] = refreshToken;
        }
        else
        {
            throw new Exception("Must provide code or refresh token");
        }

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + client_auth);

            var response = await httpClient.PostAsync(token_url, new StringContent(FormEncode(form), Encoding.UTF8, "application/x-www-form-urlencoded"));
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                TokenResponse parsed =  JsonUtility.FromJson<TokenResponse>(result);
                return parsed;
            }
            else
            {
                var result = await response.Content.ReadAsStringAsync();
                Debug.LogError($"Server responded with status code: {response.StatusCode}");
                Debug.LogError(result);
                throw new Exception(result);
            }
        }
    }

    static string FormEncode(Dictionary<string, string> pairs)
    {
        StringBuilder builder = new StringBuilder();

        foreach (var pair in pairs)
        {
            if (builder.Length > 0)
                builder.Append('&');

            builder.Append(Uri.EscapeDataString(pair.Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(pair.Value));
        }

        return builder.ToString();
    }
}

public class SimpleHTTPServer
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly Thread _serverThread;

    public SimpleHTTPServer(string prefix)
    {
        _listener.Prefixes.Add(prefix);

        _serverThread = new Thread(StartListener)
        {
            IsBackground = true
        };
        _serverThread.Start();
    }

    public void StartListener()
    {
        _listener.Start();
        while (true)
        {
            try
            {
                var context = _listener.GetContext();
                Process(context);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Listener closed")) {
                    Debug.LogError(ex.Message);
                }
                return;
            }
        }
    }

    public void Process(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath;
        if (path == "/ping")
        {
            var responseString = "pong";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        } else if (path == "/blobby/return") {
            SpotifyAuthClient.redirectRoute(context.Request);
            var responseString = "Please Return to Game";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }

    public void Stop()
    {
        _listener.Stop();
        _listener.Close();
    }
}
