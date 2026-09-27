using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

/// <summary>
/// Gestor de Unity Gaming Services.
/// Coloca este script en un GameObject vacio llamado "GestorUGS" en la escena.
/// Requiere los paquetes: Authentication SDK y Cloud Save SDK (Package Manager).
/// </summary>
public class GestorUGS : MonoBehaviour
{
    public static GestorUGS Instancia { get; private set; }

    private const string CLAVE_HISTORIAL = "historial_disparos";

    private HistorialDisparos _historial  = new HistorialDisparos();
    private bool _inicializado            = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await InicializarAsync();
    }

    // ── Inicializacion ────────────────────────────────────────────────────────

    private async Task InicializarAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _inicializado = true;
            Debug.Log("[UGS] Listo. Player ID: " + AuthenticationService.Instance.PlayerId);

            await CargarHistorialAsync();
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UGS] Error al inicializar: " + e.Message);
        }
    }

    // ── API publica ───────────────────────────────────────────────────────────

    /// <summary>Guarda un tiro en Cloud Save. Llamar desde ReporteTiro.</summary>
    public async Task GuardarTiroAsync(RegistroTiro tiro)
    {
        if (!_inicializado)
        {
            Debug.LogWarning("[UGS] No inicializado — tiro no guardado.");
            return;
        }

        _historial.disparos.Add(tiro);

        try
        {
            string json = JsonUtility.ToJson(_historial);
            var datos = new Dictionary<string, object> { { CLAVE_HISTORIAL, json } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(datos);
            Debug.Log("[UGS] Tiro #" + tiro.numero + " guardado. Total: " + _historial.disparos.Count);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UGS] Error al guardar tiro: " + e.Message);
            // Revertir para no perder sincronizacion
            _historial.disparos.Remove(tiro);
        }
    }

    /// <summary>Devuelve la lista de tiros (recarga desde Cloud Save primero).</summary>
    public async Task<List<RegistroTiro>> ObtenerHistorialAsync()
    {
        await CargarHistorialAsync();
        return _historial.disparos;
    }

    // ── Privados ──────────────────────────────────────────────────────────────

    private async Task CargarHistorialAsync()
    {
        if (!_inicializado) return;

        try
        {
            var claves    = new HashSet<string> { CLAVE_HISTORIAL };
            var resultado = await CloudSaveService.Instance.Data.Player.LoadAsync(claves);

            if (resultado.TryGetValue(CLAVE_HISTORIAL, out var valor))
            {
                string json = valor.Value.GetAs<string>();
                _historial  = JsonUtility.FromJson<HistorialDisparos>(json) ?? new HistorialDisparos();
                Debug.Log("[UGS] Historial cargado: " + _historial.disparos.Count + " tiros.");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UGS] Error al cargar historial: " + e.Message);
        }
    }
}
