using System;
using System.Collections;
using UnityEngine;

public class KPITracker : MonoBehaviour
{
    public static KPITracker Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegistrarVistaHeroe(Heroe h) => StartCoroutine(InsertarVista("Heroe", h.ID_Heroe, h.Nombre, h.ID_Heroe));

    public void RegistrarVistaSkin(Skins s)
    {
        string nombre = s.Colaboracion ? $"Collab Skin {s.ID_Skin}" : $"Base Skin {s.ID_Skin}";
        StartCoroutine(InsertarVista("Skin", s.ID_Skin, nombre, s.ID_Heroe));
    }

    public void RegistrarVistaUlti(Ultimates u) =>
        StartCoroutine(InsertarVista("Ultimate", u.ID_Ultimate, u.Nombre, u.ID_Heroe));

    private IEnumerator InsertarVista(string tipo, int idEntidad, string nombre, int idHeroe)
    {
        string body = "{" +
                        $"\"Tipo\":\"{tipo}\"," +
                        $"\"ID_Entidad\":{idEntidad}," +
                        $"\"Nombre\":\"{EscaparJson(nombre)}\"," +
                        $"\"ID_Heroe\":{idHeroe}" +
                        "}";

        yield return StartCoroutine(
            SupabaseClient.Post(
                table: "KPI_Vistas",
                body: body,
                onSuccess: _ => { },
                onError: err => Debug.LogWarning($"[KPITracker] Error: {err}")
            )
        );
    }

    private string EscaparJson(string s) => string.IsNullOrEmpty(s) ? "" : s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}