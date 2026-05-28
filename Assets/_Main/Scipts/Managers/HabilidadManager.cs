using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HabilidadManager : MonoBehaviour
{
    public Image miImagenUI;
    void Start()
    {
        StartCoroutine(CargarHabilidad());
    }

    IEnumerator CargarHabilidad()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Habilidad",
                query: "select=ID_Habilidad,Nombre,URL_Habilidad,Descripcion,Cooldown,Damage,Cura",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    var data = JsonUtility.FromJson<Wrapper<Habilidad>>(wrapped);

                    foreach (var h in data.items)
                    {
                        Debug.Log($"ID Habilidad[{h.ID_Habilidad}], nombre: {h.Nombre} — Cooldown: {h.Cooldown}, Daño: {h.Damage}, Cura: {h.Cura}");
                        Debug.Log($"  Descripción: {h.Descripcion}");
                        Debug.Log($"  Imagen: {h.URL_Habilidad}");
                        StartCoroutine(CargarImagen(h.URL_Habilidad, miImagenUI));
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
