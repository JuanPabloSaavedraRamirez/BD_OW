using System.Collections;
using UnityEngine;

public class HabilidadesManager : MonoBehaviour
{
    public static Habilidades[] TodosLosPaquetes { get; private set; }
    public static Habilidad[] TodasLasHabilidades { get; private set; }

    public bool CargaCompleta { get; private set; } = false;

    void Start() => StartCoroutine(CargarTodo());

    private IEnumerator CargarTodo()
    {
        yield return StartCoroutine(CargarPaquetes());
        yield return StartCoroutine(CargarHabilidades());
        CargaCompleta = true;
    }

    private IEnumerator CargarPaquetes()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Habilidades",
                query: "select=ID_Habilidades,ID_Heroe,ID_Habilidad_1,ID_Habilidad_2,ID_Habilidad_3,ID_Habilidad_4",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    TodosLosPaquetes = JsonUtility.FromJson<Wrapper<Habilidades>>(wrapped).items;
                },
                onError: err => Debug.LogError($"[HabilidadesManager] Error paquetes: {err}")
            )
        );
    }

    private IEnumerator CargarHabilidades()
    {
        yield return StartCoroutine(
            SupabaseClient.Get(
                table: "Habilidad",
                query: "select=ID_Habilidad,Nombre,URL_Habilidad,Descripcion,Cooldown,Damage,Cura",
                onSuccess: json =>
                {
                    string wrapped = $"{{\"items\":{json}}}";
                    TodasLasHabilidades = JsonUtility.FromJson<Wrapper<Habilidad>>(wrapped).items;
                },
                onError: err => Debug.LogError($"[HabilidadesManager] Error habilidades: {err}")
            )
        );
    }

    [System.Serializable]
    class Wrapper<T> { public T[] items; }
}