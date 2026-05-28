using System;
using System.Collections;
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
}