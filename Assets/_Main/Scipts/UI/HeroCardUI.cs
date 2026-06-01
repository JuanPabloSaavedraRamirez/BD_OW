using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HeroCardUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI heroNameText;
    [SerializeField] private Image heroImage;
    [SerializeField] private Button verDetalleButton; 

    [Header("Placeholder")]
    [SerializeField] private Sprite loadingSprite;

    private Heroe _heroe;

    public void Setup(Heroe heroe)
    {
        _heroe = heroe;

        if (heroNameText != null)
            heroNameText.text = heroe.Nombre;

        if (!string.IsNullOrEmpty(heroe.URL_Heroe)) StartCoroutine(CargarImagen(heroe.URL_Heroe));
        else if (loadingSprite != null && heroImage != null) heroImage.sprite = loadingSprite;

        verDetalleButton?.onClick.AddListener(() => HeroDetailPanel.Instance?.Mostrar(_heroe));
    }

    private IEnumerator CargarImagen(string url)
    {
        if (loadingSprite != null && heroImage != null) heroImage.sprite = loadingSprite;

        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(request);
                if (heroImage != null) heroImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else Debug.LogWarning($"[HeroCardUI] No se pudo cargar imagen: {url}\n{request.error}");
        }
    }
}