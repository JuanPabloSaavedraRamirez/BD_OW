using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HeroesManager : MonoBehaviour
{
    public Image miImagenUI;
    void Start()
    {
        StartCoroutine(CargarHeroes());
    }

    IEnumerator CargarHeroes()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Heroe",
                query: "select=ID_Heroe,Nombre,Descripcion,URL_Heroe,Vida,Escudo,Armadura,Edad,Altura",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    var data = JsonUtility.FromJson<Wrapper<Heroe>>(wrapped);

                    foreach (var h in data.items)
                    {
                        Debug.Log($"[{h.ID_Heroe}] {h.Nombre} — Vida: {h.Vida}, Escudo: {h.Escudo}, Armadura: {h.Armadura}, Edad: {h.Edad}, Altura: {h.Altura}");
                        Debug.Log($"  Descripción: {h.Descripcion}");
                        Debug.Log($"  Imagen: {h.URL_Heroe}");
                        StartCoroutine(CargarImagen(h.URL_Heroe, miImagenUI));
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