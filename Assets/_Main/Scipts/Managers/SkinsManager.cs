using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SkinsManager : MonoBehaviour
{
    public Image miImagenUI;
    void Start()
    {
        StartCoroutine(CargarSkins());
    }

    IEnumerator CargarSkins()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Skins",
                query: "select=ID_Skin,ID_Heroe,URL_Skin,Colaboracion",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    var data = JsonUtility.FromJson<Wrapper<Skins>>(wrapped);

                    foreach (var s in data.items)
                    {
                        Debug.Log($"ID Skin: [{s.ID_Skin}] -- ID Heroe: {s.ID_Heroe} — Colaboración: {s.Colaboracion}");
                        Debug.Log($"Imagen: {s.URL_Skin}");
                        StartCoroutine(CargarImagen(s.URL_Skin, miImagenUI));
                    }
                },
                onError: err => Debug.LogError($"Error al cargar skins: {err}")
            )
        );
    }

    [System.Serializable]
    class Wrapper<T> 
    { 
        public T[] items; 
    }

    IEnumerator CargarImagen(string url, Image imagenUI)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var tex     = DownloadHandlerTexture.GetContent(req);
            imagenUI.sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
