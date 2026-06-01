using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabilidadCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nombreText;
    [SerializeField] private Button boton;

    private Habilidad _habilidad;

    public void Setup(Habilidad habilidad)
    {
        _habilidad = habilidad;
        nombreText.text = habilidad.Nombre;
        boton.onClick.AddListener(() => HabilidadDetailPanel.Instance?.Mostrar(_habilidad));
    }
}