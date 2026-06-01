using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SkinCardUI : MonoBehaviour
{
    [SerializeField] private Image           skinImage;
    [SerializeField] private TextMeshProUGUI skinColaboracion;

    private Skins  _skin;
    private string _urlPendiente;

    public void Setup(Skins skin)
    {
        _skin         = skin;
        _urlPendiente = skin.URL_Skin;
        skinColaboracion.text = skin.Colaboracion ? "Colaboración" : "Base";
        KPITracker.Instance?.RegistrarVistaSkin(skin);
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(_urlPendiente))
            StartCoroutine(CargarImagen(_urlPendiente));
    }

    private IEnumerator CargarImagen(string url)
    {
        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                byte[] bytes = req.downloadHandler.data;

                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(bytes) && skinImage != null)
                {
                    skinImage.sprite = Sprite.Create(
                        tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
                else
                    Debug.LogWarning($"[SkinCardUI] No se pudo decodificar la imagen: {url}");
            }
            else
                Debug.LogWarning($"[SkinCardUI] Error descargando: {req.error} — {url}");
        }
    }
}