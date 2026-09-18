using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager singleton del reporte de tiro.
/// Coloca este script en un GameObject vacio llamado "ReporteTiro" en la escena.
/// </summary>
public class ReporteTiro : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static ReporteTiro Instancia { get; private set; }

    // ── Referencias UI (asignar en el Inspector) ───────────────────────────────
    [Header("Panel de Reporte")]
    public GameObject panelReporte;          // Panel que se activa al terminar el tiro

    [Header("Textos del Reporte (TextMeshPro)")]
    public TextMeshProUGUI textoTiempoVuelo;
    public TextMeshProUGUI textoPuntoImpacto;
    public TextMeshProUGUI textoImpulso;
    public TextMeshProUGUI textoBloquesDerribados;

    [Header("Boton de cierre")]
    public Button botonCerrar;               // Boton dentro del panel para cerrarlo

    // ── Estado interno ─────────────────────────────────────────────────────────
    private int   _bloquesDerribados = 0;
    private bool  _tiroActivo        = false;

    // ── Lifecycle ──────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
    }

    void Start()
    {
        if (panelReporte != null) panelReporte.SetActive(false);
        if (botonCerrar  != null) botonCerrar.onClick.AddListener(CerrarPanel);
    }

    // ── API publica (llamada desde otros scripts) ──────────────────────────────

    /// <summary>
    /// Llamar desde CannonController justo antes de instanciar la bala.
    /// Reinicia el contador de bloques y oculta el panel anterior.
    /// </summary>
    public void IniciarTiro()
    {
        _bloquesDerribados = 0;
        _tiroActivo        = true;
        if (panelReporte != null) panelReporte.SetActive(false);
    }

    /// <summary>
    /// Llamar desde BloqueObjetivo cada vez que un cubo toca el suelo.
    /// Solo cuenta si hay un tiro activo para no mezclar disparos.
    /// </summary>
    public void RegistrarBloqueDerribado()
    {
        if (_tiroActivo) _bloquesDerribados++;
    }

    /// <summary>
    /// Llamar desde Bala.OnCollisionEnter con los datos del primer impacto.
    /// </summary>
    public void MostrarReporte(float tiempoVuelo, Vector3 puntoImpacto, float impulso)
    {
        _tiroActivo = false;
        StartCoroutine(EsperarYMostrar(tiempoVuelo, puntoImpacto, impulso));
    }

    // ── Privados ───────────────────────────────────────────────────────────────

    /// Espera 1.5 s para que la fisica termine de procesar los cubos derribados.
    private IEnumerator EsperarYMostrar(float tiempoVuelo, Vector3 puntoImpacto, float impulso)
    {
        yield return new WaitForSeconds(1.5f);

        if (textoTiempoVuelo != null)
            textoTiempoVuelo.text = $"Tiempo de vuelo: {tiempoVuelo:F2} s";

        if (textoPuntoImpacto != null)
            textoPuntoImpacto.text =
                $"Punto de impacto: ({puntoImpacto.x:F2}, {puntoImpacto.y:F2}, {puntoImpacto.z:F2})";

        if (textoImpulso != null)
            textoImpulso.text = $"Impulso: {impulso:F2} N*s";

        if (textoBloquesDerribados != null)
            textoBloquesDerribados.text = $"Bloques derribados: {_bloquesDerribados}";

        if (panelReporte != null) panelReporte.SetActive(true);
    }

    private void CerrarPanel()
    {
        if (panelReporte != null) panelReporte.SetActive(false);
    }
}
