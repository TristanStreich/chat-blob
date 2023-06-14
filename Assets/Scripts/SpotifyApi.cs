namespace SpotifyApi {
    
    [System.Serializable]
    public class RecentlyPlayedResponse
    {
        public string href;
        public int limit;
        public string next;
        public Cursors cursors;
        public int total;
        public Item[] items;
    }

    [System.Serializable]
    public class PlaybackResponse
    {
        public Device device;
        public string repeat_state;
        public bool shuffle_state;
        public Context context;
        public long timestamp;
        public int progress_ms;
        public bool is_playing;
        public Track item;
        public string currently_playing_type;
        public Actions actions;
    }

    [System.Serializable]
    public class Device
    {
        public string id;
        public bool is_active;
        public bool is_private_session;
        public bool is_restricted;
        public string name;
        public string type;
        public int volume_percent;
    }

    [System.Serializable]
    public class Actions
    {
        public bool interrupting_playback;
        public bool pausing;
        public bool resuming;
        public bool seeking;
        public bool skipping_next;
        public bool skipping_prev;
        public bool toggling_repeat_context;
        public bool toggling_shuffle;
        public bool toggling_repeat_track;
        public bool transferring_playback;
    }


    [System.Serializable]
    public class Cursors
    {
        public string after;
        public string before;
    }

    [System.Serializable]
    public class Item
    {
        public Track track;
        public string played_at;
        public Context context;
    }

    [System.Serializable]
    public class Track
    {
        public Album album;
        public Artist[] artists;
        public string[] available_markets;
        public int disc_number;
        public int duration_ms;
        public ExternalIds external_ids;
        public ExternalUrls external_urls;
        public string href;
        public string id;
        public bool is_playable;
        public LinkedFrom linked_from;
        public Restrictions restrictions;
        public string name;
        public int popularity;
        public string preview_url;
        public int track_number;
        public string type;
        public string uri;
        public bool is_local;
        public override bool Equals(object obj) {
            return obj is Track track && id == track.id;
        }
        public override int GetHashCode() {
            return id?.GetHashCode() ?? 0;
        }
        public static bool operator ==(Track left, Track right) {
            if (ReferenceEquals(left, null)) {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(Track left, Track right) {
            return !(left == right);
        }
    }

    [System.Serializable]
    public class TrackAudioFeatures
    {
        public double acousticness;
        public string analysis_url;
        public double danceability;
        public int duration_ms;
        public double energy;
        public string id;
        public double instrumentalness;
        public int key;
        public double liveness;
        public double loudness;
        public int mode;
        public double speechiness;
        public double tempo;
        public int time_signature;
        public string track_href;
        public string type;
        public string uri;
        public double valence;
    }


    [System.Serializable]
    public class Album
    {
        public string album_type;
        public int total_tracks;
        public string[] available_markets;
        public ExternalUrls external_urls;
        public string href;
        public string id;
        public Image[] images;
        public string name;
        public string release_date;
        public string release_date_precision;
        public Restrictions restrictions;
        public string type;
        public string uri;
        public Copyright[] copyrights;
        public ExternalIds external_ids;
        public string[] genres;
        public string label;
        public int popularity;
        public string album_group;
        public Artist[] artists;
    }

    [System.Serializable]
    public class ExternalUrls
    {
        public string spotify;
    }

    [System.Serializable]
    public class Image
    {
        public string url;
        public int height;
        public int width;
    }

    [System.Serializable]
    public class Restrictions
    {
        public string reason;
    }

    [System.Serializable]
    public class Copyright
    {
        public string text;
        public string type;
    }

    [System.Serializable]
    public class ExternalIds
    {
        public string isrc;
        public string ean;
        public string upc;
    }

    [System.Serializable]
    public class Artist
    {
        public ExternalUrls external_urls;
        public Followers followers;
        public string[] genres;
        public string href;
        public string id;
        public Image[] images;
        public string name;
        public int popularity;
        public string type;
        public string uri;
    }

    [System.Serializable]
    public class Followers
    {
        public string href;
        public int total;
    }

    [System.Serializable]
    public class Context
    {
        public string type;
        public string href;
        public ExternalUrls external_urls;
        public string uri;
    }

    [System.Serializable]
    public class LinkedFrom
    {
        // additional fields can go here
    }

}