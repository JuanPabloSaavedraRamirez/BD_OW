using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HabilidadDetailPanel : MonoBehaviour
{
    public static HabilidadDetailPanel Instance { get; private set; }

    [Header("Panel raíz")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;

    [Header("Info de la habilidad")]
    [SerializeField] private Image habilidadImage;
    [SerializeField] private TextMeshProUGUI habilidadNombre;
    [SerializeField] private TextMeshProUGUI habilidadDescripcion;
    [SerializeField] private TextMeshProUGUI habilidadStats;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        closeButton?.onClick.AddListener(Cerrar);
        panelRoot.SetActive(false);
    }

    public void Mostrar(Habilidad h)
    {
        KPITracker.Instance?.RegistrarVistaHabilidad(h);

        habilidadNombre.text = h.Nombre;
        habilidadDescripcion.text = h.Descripcion;
        habilidadStats.text = $"⏱ Cooldown: {h.Cooldown}s   💥 Daño: {h.Damage}   💚 Cura: {h.Cura}";

        if (!string.IsNullOrEmpty(h.URL_Habilidad))
            StartCoroutine(CargarImagen(h.URL_Habilidad));

        panelRoot.SetActive(true);
    }

    public void Cerrar() => panelRoot.SetActive(false);

    private IEnumerator CargarImagen(string url)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success && habilidadImage != null)
        {
            var tex = DownloadHandlerTexture.GetContent(req);
            habilidadImage.sprite = Sprite.Create(
                tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}