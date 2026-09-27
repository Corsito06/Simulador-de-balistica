using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de historial de disparos. Carga los datos desde GestorUGS y los muestra en una lista.
/// Coloca este script en un GameObject del Canvas (puede ser el mismo Reportes u otro separado).
/// </summary>
public class PanelHistorial : MonoBehaviour
{
    public static PanelHistorial Instancia { get; private set; }

    [Header("Panel principal")]
    public GameObject panelHistorial;       // Panel con el ScrollView
    public Button     btnVerHistorial;      // Boton en el menu principal para abrir el historial

    [Header("Contenido del ScrollView")]
    public Transform  contenedorLista;      // El Transform Content del ScrollView
    public TextMeshProUGUI textoTitulo;     // Titulo del panel

    [Header("Boton de cierre")]
    public Button btnCerrarHistorial;

    private List<GameObject> _filas = new List<GameObject>();

    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;

        // Registrar listeners en Awake (corre aunque el panel empiece inactivo)
        if (panelHistorial     != null) panelHistorial.SetActive(false);
        if (btnVerHistorial    != null) btnVerHistorial.onClick.AddListener(AbrirHistorial);
        if (btnCerrarHistorial != null) btnCerrarHistorial.onClick.AddListener(CerrarHistorial);
    }

    // ── API publica ───────────────────────────────────────────────────────────

    public async void AbrirHistorial()
    {
        LimpiarFilas();
        if (textoTitulo  != null) textoTitulo.text = "Cargando...";
        if (panelHistorial != null) panelHistorial.SetActive(true);

        if (GestorUGS.Instancia == null)
        {
            CrearFila("<color=#ff6b6b>UGS no inicializado. Revisa la configuracion del proyecto.</color>");
            if (textoTitulo != null) textoTitulo.text = "Historial — Error";
            return;
        }

        List<RegistroTiro> disparos = await GestorUGS.Instancia.ObtenerHistorialAsync();

        if (disparos.Count == 0)
        {
            CrearFila("No hay disparos registrados aun.");
            if (textoTitulo != null) textoTitulo.text = "Historial (0 tiros)";
            return;
        }

        if (textoTitulo != null) textoTitulo.text = $"Historial — {disparos.Count} tiros";

        // Mostrar del mas reciente al mas antiguo
        for (int i = disparos.Count - 1; i >= 0; i--)
        {
            RegistroTiro d      = disparos[i];
            string estado = d.acierto ? "<color=#69db7c>IMPACTO</color>" : "<color=#ff6b6b>FALLO</color>";
            string linea  =
                $"<b>#{d.numero}</b>  {estado}  |  A:{d.angulo:F0}°  F:{d.fuerza:F0}m/s  M:{d.masa:F1}kg\n" +
                $"<size=11><color=#adb5bd>Vuelo:{d.tiempoVuelo:F2}s  Impulso:{d.impulso:F1}N·s  " +
                $"Dist:{d.distanciaImpacto:F1}m  Bloques:{d.bloquesDerribados}  {d.timestamp}</color></size>";

            CrearFila(linea);
        }
    }

    public void CerrarHistorial()
    {
        if (panelHistorial != null) panelHistorial.SetActive(false);
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private void CrearFila(string contenido)
    {
        var go  = new GameObject("FilaHistorial");
        go.transform.SetParent(contenedorLista, false);

        var tmp            = go.AddComponent<TextMeshProUGUI>();
        tmp.text           = contenido;
        tmp.fontSize           = 13;
        tmp.color              = Color.white;
        tmp.enableWordWrapping = true;

        var le             = go.AddComponent<UnityEngine.UI.LayoutElement>();
        le.preferredHeight = 48;
        le.flexibleWidth   = 1;

        _filas.Add(go);
    }

    private void LimpiarFilas()
    {
        foreach (var f in _filas) if (f != null) Destroy(f);
        _filas.Clear();
    }
}
