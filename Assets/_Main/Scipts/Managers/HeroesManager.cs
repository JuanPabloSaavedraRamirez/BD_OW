using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeroesManager : MonoBehaviour
{
    [Header("Lista de héroes")]
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private Transform  contentParent;

    private Heroe[] _heroes;
    private Skins[] _skins;
    private Ultimates[] _ultimates;

    void Start()
    {
        StartCoroutine(CargarTodo());
    }

    IEnumerator CargarTodo()
    {
        yield return StartCoroutine(CargarHeroes());
        yield return StartCoroutine(CargarSkins());
        yield return StartCoroutine(CargarUltimados());

        HeroDetailPanel.TodasLasSkins  = _skins;
        HeroDetailPanel.TodosLosUltis  = _ultimates;

        Debug.Log($"[HeroesManager] Carga completa — Héroes:{_heroes?.Length} Skins:{_skins?.Length} Ultis:{_ultimates?.Length}");

        foreach (var heroe in _heroes)
        {
            GameObject card = Instantiate(heroCardPrefab, contentParent);
            card.GetComponent<HeroCardUI>()?.Setup(heroe);
        }
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
                    _heroes = JsonUtility.FromJson<Wrapper<Heroe>>(wrapped).items;
                    Debug.Log($"[HeroesManager] Héroes cargados: {_heroes.Length}");
                },
                onError: err => Debug.LogError($"Error héroes: {err}")
            )
        );
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
                    _skins = JsonUtility.FromJson<Wrapper<Skins>>(wrapped).items;
                    Debug.Log($"[HeroesManager] Skins cargadas: {_skins.Length}");
                },
                onError: err => Debug.LogError($"Error skins: {err}")
            )
        );
    }

    IEnumerator CargarUltimados()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Ultimate",
                query: "select=ID_Ultimate,ID_Heroe,Nombre,URL_Ulti,Descripcion,Cooldown,Damage",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    _ultimates = JsonUtility.FromJson<Wrapper<Ultimates>>(wrapped).items;
                    Debug.Log($"[HeroesManager] Ultis cargadas: {_ultimates.Length}");
                },
                onError: err => Debug.LogError($"Error ultimates: {err}")
            )
        );
    }

    [System.Serializable]
    class Wrapper<T> { public T[] items; }
}