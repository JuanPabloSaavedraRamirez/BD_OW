using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeroesManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject heroCardPrefab;
    [SerializeField] private Transform contentParent;

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

                    foreach (var heroe in data.items)
                    {
                        GameObject card = Instantiate(heroCardPrefab, contentParent);
                        HeroCardUI cardUI = card.GetComponent<HeroCardUI>();

                        if (cardUI != null) cardUI.Setup(heroe);
                        else Debug.LogError("[HeroesManager] El prefab no tiene el componente HeroCardUI.");
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
}