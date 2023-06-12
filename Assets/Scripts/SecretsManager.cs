using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;

/// Interface for managing secrets that we have to manually put in only once.
/// this is a temporary solution and will not appear in the final game
public class SecretsManager : MonoBehaviour
{

    public GameObject newCanvas;
    public TMP_InputField newTextInput;
    public Text newDisplayText;

    public Button button;

    public static string SecretsDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.blobby";

    public static SecretsManager Manager = null;

    public static string GPTAPIKey {
        get {
            return getOrInitKey(Secret.gpt_api_key, ref _GPTAPIKey);
        }
    }
    private static string _GPTAPIKey = null;

    public static string SpotifyClientSecret {
        get {
            return getOrInitKey(Secret.spotify_client_secret, ref _SpotifyClientSecret);
        }
    }

    private static string _SpotifyClientSecret = null;

    private void Awake()
    {
        if (Manager == null)
        {
            Manager = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // init_pop_up();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum Secret {
        gpt_api_key,
        spotify_client_secret
    }

    static string getOrInitKey(Secret secret, ref string private_var) {
        if (private_var != null) {
            return private_var;
        } else {
            string val = getFromFile(secret);
            private_var = val;
            return val;
        }
    }

    static string getFromFile(Secret secret) {
        string path = getPath(secret);
        try {
            var output = File.ReadAllText(path);
            return output;
        } catch (Exception ex) when (ex is FileNotFoundException ||
                               ex is DirectoryNotFoundException)
        {
            getFromPopUp(secret);
            return "NOTAKEY";
        }

    }

    public static string getPath(Secret secret) {
        return SecretsDir + "/" + secret.ToString();
    }

    static void saveSecret(Secret secret, string val) {
        string path = getPath(secret);
        // init dir if it is not there
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        // write to file and create file if not there
        File.WriteAllText(path, val);
    }


    // opens pop up to get secret and then saves secret to file after pop up is done. 
    // The popup happens async. This will raise exception to halt codepath of wherever the secret was fetched
    // this only happens once and this should be fine. Also just for dev purposes only.
    // The entire secrets manager needs to go
    static void getFromPopUp(Secret secret) {
        Manager.button.onClick.RemoveAllListeners();
        Manager.button.onClick.AddListener(delegate() {
            string value = Manager.newTextInput.text;
            saveSecret(secret, value);
            Manager.ShowRestartMessage();
        });
        Manager.ShowPopup("Please input the secret: " + secret.ToString());
    }

    public void ShowPopup(string message)
    {
        newDisplayText.text = message;
        newCanvas.SetActive(true);
    }

    public void ShowRestartMessage()
    {
        button.gameObject.SetActive(false);
        newTextInput.gameObject.SetActive(false);
        newDisplayText.text = "Please Restart the Game. I know I know, but this is just for dev purposes. This entire GUI will not appear in final game";
    }
}