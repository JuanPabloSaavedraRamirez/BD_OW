using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HeroDetailPanel : MonoBehaviour
{
    public static HeroDetailPanel Instance { get; private set; }

    public static Skins[] TodasLasSkins  { get; set; }
    public static Ultimates[] TodosLosUltis  { get; set; }

    [Header("Panel raíz")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;

    [Header("Info del héroe")]
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroDescripcion;
    [SerializeField] private TextMeshProUGUI heroStats;

    [Header("Skins")]
    [SerializeField] private Transform skinsContent;
    [SerializeField] private GameObject skinCardPrefab;

    [Header("Ultimate")]
    [SerializeField] private Image ultiImage;
    [SerializeField] private TextMeshProUGUI ultiNombre;
    [SerializeField] private TextMeshProUGUI ultiDescripcion;
    [SerializeField] private TextMeshProUGUI ultiStats;

    [Header("Habilidades — 4 botones fijos")]
    [SerializeField] private GameObject habilidadCardPrefab;
    [SerializeField] private Transform habilidadesContent; 

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        closeButton?.onClick.AddListener(Cerrar);
        panelRoot.SetActive(false);
    }

    public void Mostrar(Heroe heroe)
    {
        KPITracker.Instance?.RegistrarVistaHeroe(heroe);

        heroName.text = heroe.Nombre;
        heroDescripcion.text = heroe.Descripcion;
        heroStats.text = $"❤ {heroe.Vida}   🛡 {heroe.Escudo}   🔰 {heroe.Armadura}   Edad: {heroe.Edad}   Altura: {heroe.Altura:F1}m";
        StartCoroutine(CargarImagen(heroe.URL_Heroe, heroImage));

        CargarSkins(heroe.ID_Heroe);
        CargarUltimate(heroe.ID_Heroe);
        CargarHabilidades(heroe.ID_Heroe);

        panelRoot.SetActive(true);
    }

    public void Cerrar() => panelRoot.SetActive(false);

    private void CargarSkins(int idHeroe)
    {
        foreach (Transform child in skinsContent) Destroy(child.gameObject);
        if (TodasLasSkins == null) return;

        foreach (var skin in TodasLasSkins.Where(s => s.ID_Heroe == idHeroe))
        {
            var card = Instantiate(skinCardPrefab, skinsContent);
            card.GetComponent<SkinCardUI>()?.Setup(skin);
        }
    }


    private void CargarUltimate(int idHeroe)
    {
        if (TodosLosUltis == null) return;

        var ulti = TodosLosUltis.FirstOrDefault(u => u.ID_Heroe == idHeroe);
        if (ulti == null)
        {
            ultiNombre.text = "Sin ultimate";
            ultiDescripcion.text = ultiStats.text = "";
            return;
        }

        KPITracker.Instance?.RegistrarVistaUlti(ulti);
        ultiNombre.text = ulti.Nombre;
        ultiDescripcion.text = ulti.Descripcion;
        ultiStats.text = $"⏱ {ulti.Cooldown}s   💥 {ulti.Damage}";
        StartCoroutine(CargarImagen(ulti.URL_Ulti, ultiImage));
    }

    private void CargarHabilidades(int idHeroe)
    {
        foreach (Transform child in habilidadesContent) Destroy(child.gameObject);

        if (HabilidadesManager.TodosLosPaquetes == null ||
            HabilidadesManager.TodasLasHabilidades == null) return;

        var paquete = HabilidadesManager.TodosLosPaquetes
            .FirstOrDefault(p => p.ID_Heroe == idHeroe);

        if (paquete == null) return;

        int[] ids = new[]
        {
            paquete.ID_Habilidad_1,
            paquete.ID_Habilidad_2,
            paquete.ID_Habilidad_3,
            paquete.ID_Habilidad_4
        };

        foreach (int idHab in ids)
        {
            var hab = HabilidadesManager.TodasLasHabilidades
                .FirstOrDefault(h => h.ID_Habilidad == idHab);

            if (hab == null) continue;

            var card = Instantiate(habilidadCardPrefab, habilidadesContent);
            card.GetComponent<HabilidadCardUI>()?.Setup(hab);
        }
    }

    private IEnumerator CargarImagen(string url, Image target)
    {
        if (string.IsNullOrEmpty(url) || target == null) yield break;

        using var req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var tex = DownloadHandlerTexture.GetContent(req);
            target.sprite = Sprite.Create(
                tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}