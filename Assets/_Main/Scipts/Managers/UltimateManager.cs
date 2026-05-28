using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UltimateManager : MonoBehaviour
{
    public Image miImagenUI;
    void Start()
    {
        StartCoroutine(CargarUltimates());
    }

    IEnumerator CargarUltimates()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Ultimate",
                query: "select=ID_Ultimate, ID_Heroe,Nombre,URL_Ulti,Descripcion,Cooldown,Damage",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    var data = JsonUtility.FromJson<Wrapper<Ultimates>>(wrapped);

                    foreach (var u in data.items)
                    {
                        Debug.Log($"ID Ultimate[{u.ID_Ultimate}] ID Heroe: {u.ID_Heroe}, nombre: {u.Nombre} — Cooldown: {u.Cooldown}, Daño: {u.Damage}");
                        Debug.Log($"  Descripción: {u.Descripcion}");
                        Debug.Log($"  Imagen: {u.URL_Ulti}");
                        StartCoroutine(CargarImagen(u.URL_Ulti, miImagenUI));
                    }
                },
                onError: err => Debug.LogError($"Error al cargar héroes: {err}")
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
