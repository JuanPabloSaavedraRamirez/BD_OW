using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class HabilidadCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nombreText;
    [SerializeField] private Button boton;
    [SerializeField] private Image abilitieImage;

    [Header("Placeholder")]
    [SerializeField] private Sprite loadingSprite;

    private Habilidad _habilidad;
    private string _urlPendiente;

    public void Setup(Habilidad habilidad)
    {
        _habilidad = habilidad;
        _urlPendiente = habilidad.URL_Habilidad;

        nombreText.text = habilidad.Nombre;

        if (loadingSprite != null && abilitieImage != null)
            abilitieImage.sprite = loadingSprite;

        boton.onClick.AddListener(() => HabilidadDetailPanel.Instance?.Mostrar(_habilidad));

    }

    void Start()
    {
        if (!string.IsNullOrEmpty(_urlPendiente))
            StartCoroutine(CargarImagen(_urlPendiente));
    }

    private IEnumerator CargarImagen(string url)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(request);
                if (abilitieImage != null)
                    abilitieImage.sprite = Sprite.Create(
                        tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else
                Debug.LogWarning($"[HabilidadCardUI] No se pudo cargar imagen: {url}\n{request.error}");
        }
    }
}