using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CacheManager : MonoBehaviour {
    static string pathBase;
    Dictionary<string, Texture> texturesCache = new Dictionary<string, Texture> ();
    Dictionary<string, int> texturesRefs = new Dictionary<string, int> ();
    Dictionary<string, Mesh> meshesCache = new Dictionary<string, Mesh> ();
    public static CacheManager Instance;
    void Awake () {
        if (Instance) {
            Destroy (gameObject);
            return;
        }
        pathBase = Application.persistentDataPath + "/Cache/";
        Instance = this;
    }
    void Start () {
        SetupCacheDirectories ();
    }
    void SetupCacheDirectories () {
        if (!Directory.Exists (pathBase + "Models"))
            Directory.CreateDirectory (pathBase + "Models");
        if (!Directory.Exists (pathBase + "Textures"))
            Directory.CreateDirectory (pathBase + "Textures");
        if (!Directory.Exists (pathBase + "RawImages"))
            Directory.CreateDirectory (pathBase + "RawImages");
    }
    public static bool IsCashed (string url, FileType type) {
        return File.Exists (GetFilePath (url, type));
    }
    public static void Cache (FileType type, string url, byte[] data) {
        string path = GetFilePath (url, type);
        File.WriteAllBytes (path, data);
    }
    public static string GetCacheURI (string url, FileType type) {
        string path = GetFilePath (url, type);
        if (File.Exists (path))
            return "file:///" + path;
        return null;
    }
    public static string GetFilePath (string url, FileType type) {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ('%', '_').Replace ('?', '_');
        return GetFilePath (fileName, url, type);
    }
    public static string GetFilePath (string fileName, string url, FileType type) {
        string folder = "";
        switch (type) {
            case FileType.model:
                folder = "Models/";
                break;
            case FileType.texture:
                folder = "Textures/";
                break;
            case FileType.rawImage:
                folder = "RawImages/";
                break;
            default:
                break;
        }
        return pathBase + folder + fileName;
    }
    public void DownloadTexture (string url, FileType type, Action<Texture> callback, Action<string> callbackPath = null) {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ('%', '_').Replace ('?', '_');
        if (texturesCache.ContainsKey (fileName) && texturesCache[fileName] != null) {
            texturesRefs[fileName]++;
            callback (texturesCache[fileName]);
            callbackPath?.Invoke (GetFilePath (fileName, url, type));
        } else {
            string cachePath = GetFilePath (fileName, url, type);
            bool isCached = File.Exists (cachePath);
            if (isCached) {
                Texture2D texture = new Texture2D (2, 2);
                texture.LoadImage (File.ReadAllBytes (cachePath), true);
                texturesCache[fileName] = texture;
                if (texturesRefs.ContainsKey (fileName))
                    texturesRefs[fileName]++;
                else
                    texturesRefs[fileName] = 1;
                callback (texture);
                callbackPath?.Invoke (cachePath);
            } else {
                StartCoroutine (DownloadTexture (fileName, cachePath, url, callback));
            }
        }
    }

    IEnumerator DownloadTexture (string fileName, string cachePath, string url, Action<Texture> callback) {
        if (texturesCache.ContainsKey (fileName)) {
            yield return new WaitUntil (() => texturesCache[fileName] != null);
            callback (texturesCache[fileName]);
            yield break;
        }
        texturesCache.Add (fileName, null);
        texturesRefs.Add (fileName, 0);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture (url)) {
            yield return www.SendWebRequest ();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log (www.error + ":" + url);
            } else {
                Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                texturesCache[fileName] = texture;
                texturesRefs[fileName]++;
                callback (texture);
                try {
                    File.WriteAllBytes (cachePath, www.downloadHandler.data);
                } catch (Exception) {
                }
            }
        }
    }
    public void DestroyTexture (string url) {
        if (string.IsNullOrEmpty (url))
            return;
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ('%', '_').Replace ('?', '_');
        if (texturesCache.ContainsKey (fileName)) {
            int refs = (--texturesRefs[fileName]);
            if (refs == 0) {
                DestroyImmediate (texturesCache[fileName]);
                texturesCache.Remove (fileName);
                texturesRefs.Remove (fileName);
            }
        }
    }

    public void DownloadMesh (string url, FileType type, Action<Mesh> callback) {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ('%', '_').Replace ('?', '_');
        if (meshesCache.ContainsKey (fileName) && meshesCache[fileName] != null) {
            callback (meshesCache[fileName]);
            return;
        } else {
            string cachePath = GetFilePath (fileName, url, type);
            bool isCached = File.Exists (cachePath);
            if (isCached) {
                Mesh mesh = MeshSerializer.ReadMesh (File.ReadAllBytes (cachePath));
                meshesCache.Add (fileName, mesh);
                callback (mesh);
                return;
            } else {
                StartCoroutine (DownloadMesh (fileName, cachePath, url, callback));
            }
        }
    }

    IEnumerator DownloadMesh (string fileName, string cachePath, string url, Action<Mesh> callback) {
        if (meshesCache.ContainsKey (fileName)) {
            yield return new WaitUntil (() => meshesCache[fileName] != null);
            callback (meshesCache[fileName]);
            yield break;
        }
        meshesCache.Add (fileName, null);
        using (UnityWebRequest www = UnityWebRequest.Get (url)) {
            yield return www.SendWebRequest ();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log (www.error + ":" + url);
            } else {
                Mesh mesh = MeshSerializer.ReadMesh (www.downloadHandler.data);
                meshesCache[fileName] = mesh;
                callback (mesh);
                File.WriteAllBytes (cachePath, www.downloadHandler.data);
            }
        }
    }
    public void DestroyMesh (string url) {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ('%', '_').Replace ('?', '_');
        if (meshesCache.ContainsKey (fileName)) {
            DestroyImmediate (meshesCache[fileName]);
            meshesCache.Remove (fileName);
        }
    }
    public void DownloadTempMedia (string url, string filename, Action<string> callback) {
        StartCoroutine (DownloadMediaCor (url, filename, callback));
    }
    IEnumerator DownloadMediaCor (string url, string filename, Action<string> callback) {
        string filePath = Path.Combine (Application.temporaryCachePath, filename);
        if (File.Exists (filePath)) {
            callback (filePath);
            yield break;
        }
        UnityWebRequest www = UnityWebRequest.Get (url);
        yield return www.SendWebRequest ();
        if (www.result != UnityWebRequest.Result.Success) {
            callback (null);
        } else {
            try {
                File.WriteAllBytes (filePath, www.downloadHandler.data);
                callback (filePath);
            } catch (Exception e) {
                Debug.LogError (e.StackTrace);
                callback (null);
            }
        }
    }
}
public enum FileType {
    model,
    texture,
    rawImage
}
