using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Singleton central que recibe datos de cada disparo, calcula la puntuacion
/// y muestra el reporte de tiro en pantalla.
/// </summary>
public class RegistroDisparo : MonoBehaviour
{
    // ------------------------------------------------------------------ Singleton
    public static RegistroDisparo Instancia { get; private set; }

    // ------------------------------------------------------------------ Datos internos
    [System.Serializable]
    public struct DatosTiro
    {
        public int     numeroDisparo;
        public float   tiempoVuelo;
        public Vector3 puntoImpacto;
        public float   velocidadRelativa;
        public float   impulso;
        public float   angulo;
        public float   fuerza;
        public float   masa;
        public int     piezasDerribadas;
        public float   puntuacion;
        public string  objetoImpactado;
    }

    private readonly List<DatosTiro> _historial = new List<DatosTiro>();
    private int _shotNumberActual;

    // ------------------------------------------------------------------ Referencias UI
    [Header("Panel de resultados")]
    [SerializeField] private GameObject  panelResultados;
    [SerializeField] private TextMeshProUGUI textoReporte;
    [SerializeField] private Button      btnSiguienteTiro;

    [Header("Referencia a la estructura objetivo (opcional)")]
    [SerializeField] private ObjetivoPrincipal estructuraObjetivo;

    // ------------------------------------------------------------------ Lifecycle
    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
    }

    void Start()
    {
        if (panelResultados != null) panelResultados.SetActive(false);
        if (btnSiguienteTiro != null)
            btnSiguienteTiro.onClick.AddListener(CerrarPanel);
    }

    // ------------------------------------------------------------------ API publica
    /// <summary>Llamado por CannonController justo antes de instanciar la bala.</summary>
    public void IniciarTiro(int numeroDisparo)
    {
        _shotNumberActual = numeroDisparo;

        if (panelResultados != null) panelResultados.SetActive(false);

        if (estructuraObjetivo != null)
            estructuraObjetivo.IniciarConteoTiro();
    }

    /// <summary>Llamado por Bala al impactar (o al ser destruida sin impacto).</summary>
    public void RegistrarImpacto(
        int     numeroDisparo,
        float   tiempoVuelo,
        Vector3 puntoImpacto,
        float   velocidadRelativa,
        float   impulso,
        float   angulo,
        float   fuerza,
        float   masa,
        string  objetoImpactado)
    {
        // Dar un frame para que los joints se rompan y se cuenten las piezas
        StartCoroutine(RegistrarConDelay(
            numeroDisparo, tiempoVuelo, puntoImpacto,
            velocidadRelativa, impulso, angulo, fuerza, masa, objetoImpactado));
    }

    private System.Collections.IEnumerator RegistrarConDelay(
        int     numeroDisparo,
        float   tiempoVuelo,
        Vector3 puntoImpacto,
        float   velocidadRelativa,
        float   impulso,
        float   angulo,
        float   fuerza,
        float   masa,
        string  objetoImpactado)
    {
        // Esperar 3 frames para que la fisica procese los joints rotos
        yield return null;
        yield return null;
        yield return null;

        int piezasDerribadas = 0;
        if (estructuraObjetivo != null)
            piezasDerribadas = estructuraObjetivo.PiezasDerribadasEsteTiro;

        // Formula de puntuacion: base por impacto + bonus por piezas derribadas
        float puntuacion = 0f;
        if (velocidadRelativa > 0f)
            puntuacion = (velocidadRelativa * impulso) + (piezasDerribadas * 50f);

        var datos = new DatosTiro
        {
            numeroDisparo     = numeroDisparo,
            tiempoVuelo       = tiempoVuelo,
            puntoImpacto      = puntoImpacto,
            velocidadRelativa = velocidadRelativa,
            impulso           = impulso,
            angulo            = angulo,
            fuerza            = fuerza,
            masa              = masa,
            piezasDerribadas  = piezasDerribadas,
            puntuacion        = puntuacion,
            objetoImpactado   = objetoImpactado
        };

        _historial.Add(datos);
        MostrarReporte(datos);
    }

    // ------------------------------------------------------------------ UI
    private void MostrarReporte(DatosTiro d)
    {
        if (textoReporte == null) return;

        int piezasEnPie = estructuraObjetivo != null ? estructuraObjetivo.PiezasEnPie : -1;
        int totalPiezas = estructuraObjetivo != null ? estructuraObjetivo.TotalPiezas  : -1;

        string sinImpacto = d.objetoImpactado == "Sin impacto" ? " (FALLO)" : "";

        textoReporte.text =
            $"=== REPORTE DE TIRO N.{d.numeroDisparo} ===\n" +
            $"Angulo:            {d.angulo:F1} deg\n" +
            $"Fuerza:            {d.fuerza:F1} m/s\n" +
            $"Masa:              {d.masa:F1} kg\n" +
            $"Tiempo de vuelo:   {d.tiempoVuelo:F2} s\n" +
            $"Punto de impacto:  ({d.puntoImpacto.x:F1}, {d.puntoImpacto.y:F1}, {d.puntoImpacto.z:F1})\n" +
            $"Vel. relativa:     {d.velocidadRelativa:F2} m/s{sinImpacto}\n" +
            $"Impulso:           {d.impulso:F2} N*s\n" +
            $"Piezas derribadas: {d.piezasDerribadas}" +
            (totalPiezas >= 0 ? $" / {totalPiezas} (quedan {piezasEnPie})\n" : "\n") +
            $"------------------------\n" +
            $"PUNTUACION: {d.puntuacion:F0} pts";

        if (panelResultados != null) panelResultados.SetActive(true);
    }

    private void CerrarPanel()
    {
        if (panelResultados != null) panelResultados.SetActive(false);
    }

    // ------------------------------------------------------------------ Historial
    public List<DatosTiro> ObtenerHistorial() => _historial;
}
