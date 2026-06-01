using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SkinCardUI : MonoBehaviour
{
    [SerializeField] private Image skinImage;
    [SerializeField] private TextMeshProUGUI skinColaboracion;

    private Skins _skin;

    public void Setup(Skins skin)
    {
        _skin = skin;

        skinColaboracion.text = skin.Colaboracion ? "Colaboración" : "Base";

        if (!string.IsNullOrEmpty(skin.URL_Skin)) StartCoroutine(CargarImagen(skin.URL_Skin));

        KPITracker.Instance?.RegistrarVistaSkin(skin);
    }


    private IEnumerator CargarImagen(string url)
    {
        using (var req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success && skinImage != null)
            {
                var tex = DownloadHandlerTexture.GetContent(req);
                skinImage.sprite = Sprite.Create(
                    tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}