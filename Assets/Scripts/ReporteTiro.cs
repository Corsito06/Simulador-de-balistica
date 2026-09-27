using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager singleton del reporte de tiro.
/// Coloca este script en un GameObject vacio en la escena.
/// </summary>
public class ReporteTiro : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static ReporteTiro Instancia { get; private set; }

    // ── Referencias UI ────────────────────────────────────────────────────────
    [Header("Panel de Reporte")]
    public GameObject panelReporte;

    [Header("Textos del Reporte (TextMeshPro)")]
    public TextMeshProUGUI textoTiempoVuelo;
    public TextMeshProUGUI textoPuntoImpacto;
    public TextMeshProUGUI textoImpulso;
    public TextMeshProUGUI textoBloquesDerribados;

    [Header("Boton de cierre")]
    public Button botonCerrar;

    // ── Estado interno ────────────────────────────────────────────────────────
    private int     _bloquesDerribados = 0;
    private bool    _tiroActivo        = false;

    // Metadatos del tiro actual (recibidos desde Arma al disparar)
    private int     _numeroTiro;
    private float   _angulo;
    private float   _fuerza;
    private float   _masa;
    private Vector3 _posicionDisparo;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
        Debug.Log("[ReporteTiro] Singleton inicializado OK.");
    }

    void Start()
    {
        if (panelReporte != null) panelReporte.SetActive(false);
        else Debug.LogWarning("[ReporteTiro] panelReporte NO esta asignado en el Inspector.");

        if (botonCerrar != null) botonCerrar.onClick.AddListener(CerrarPanel);
        else Debug.LogWarning("[ReporteTiro] botonCerrar NO esta asignado en el Inspector.");
    }

    // ── API publica ───────────────────────────────────────────────────────────

    /// <summary>Llamar desde Arma.Shoot() antes de instanciar la bala.</summary>
    public void IniciarTiro(int numero, float angulo, float fuerza, float masa, Vector3 posDisparo)
    {
        _bloquesDerribados = 0;
        _tiroActivo        = true;
        _numeroTiro        = numero;
        _angulo            = angulo;
        _fuerza            = fuerza;
        _masa              = masa;
        _posicionDisparo   = posDisparo;

        if (panelReporte != null) panelReporte.SetActive(false);
        Debug.Log($"[ReporteTiro] IniciarTiro #{numero} — A:{angulo:F0}° F:{fuerza:F0} M:{masa:F1}");
    }

    /// <summary>Llamar desde BloqueObjetivo cuando un cubo cae.</summary>
    public void RegistrarBloqueDerribado()
    {
        if (_tiroActivo) _bloquesDerribados++;
    }

    /// <summary>Llamar desde Bala.OnCollisionEnter con los datos del impacto.</summary>
    public void MostrarReporte(float tiempoVuelo, Vector3 puntoImpacto, float impulso)
    {
        Debug.Log($"[ReporteTiro] MostrarReporte() | Vuelo:{tiempoVuelo:F2}s Impulso:{impulso:F2}");
        StartCoroutine(EsperarYMostrar(tiempoVuelo, puntoImpacto, impulso));
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private IEnumerator EsperarYMostrar(float tiempoVuelo, Vector3 puntoImpacto, float impulso)
    {
        Debug.Log("[ReporteTiro] Esperando 1.5s...");
        yield return new WaitForSeconds(1.5f);

        // Cerrar conteo de bloques
        _tiroActivo = false;

        // ── Actualizar textos UI ──────────────────────────────────────────────
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

        // ── Guardar en UGS ────────────────────────────────────────────────────
        if (GestorUGS.Instancia != null)
        {
            var tiro = new RegistroTiro
            {
                numero            = _numeroTiro,
                angulo            = _angulo,
                fuerza            = _fuerza,
                masa              = _masa,
                tiempoVuelo       = tiempoVuelo,
                impulso           = impulso,
                bloquesDerribados = _bloquesDerribados,
                acierto           = impulso > 0f,
                distanciaImpacto  = Vector3.Distance(_posicionDisparo, puntoImpacto),
                timestamp         = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            // Fire-and-forget: no bloqueamos la UI
            _ = GestorUGS.Instancia.GuardarTiroAsync(tiro);
        }
        else
        {
            Debug.LogWarning("[ReporteTiro] GestorUGS no encontrado — tiro no guardado en la nube.");
        }
    }

    public void CerrarPanel()
    {
        if (panelReporte != null) panelReporte.SetActive(false);
    }
}
