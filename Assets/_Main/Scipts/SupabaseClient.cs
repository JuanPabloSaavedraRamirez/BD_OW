using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public static class SupabaseClient
{
    public static IEnumerator Get(string table, string query, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{SupabaseConfig.Url}/rest/v1/{table}?{query}";

        using var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {SupabaseConfig.AnonKey}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success) onSuccess?.Invoke(req.downloadHandler.text);
        else onError?.Invoke(req.error);
    }

    public static IEnumerator Post(string table, string body, Action<string> onSuccess, Action<string> onError)
    {
        string url = $"{SupabaseConfig.Url}/rest/v1/{table}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type",  "application/json");
        req.SetRequestHeader("apikey",        SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {SupabaseConfig.AnonKey}");
        req.SetRequestHeader("Prefer",        "return=minimal");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success) onSuccess?.Invoke(req.downloadHandler.text);
        else onError?.Invoke(req.error);
    }
}