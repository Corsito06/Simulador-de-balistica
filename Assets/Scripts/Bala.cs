using UnityEngine;

/// <summary>
/// Componente de la bala.
/// Cronometra el vuelo y, al primer impacto contra algo que NO sea el suelo,
/// envia los datos fisicos al ReporteTiro.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bala : MonoBehaviour
{
    public int numeroDisparo; 
    // Metadatos del disparo — asignados por CannonController al instanciar
    [HideInInspector] public float anguloDisparo;
    [HideInInspector] public float fuerzaDisparo;
    [HideInInspector] public float masaDisparo;

    private float _tiempoInicio;
    private bool  _impactoRegistrado = false;

    void Start()
    {
        _tiempoInicio = Time.time;
        Debug.Log("[Bala] Instanciada. Script activo.");
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log($"[Bala] Colision con: '{col.gameObject.name}' | Tag: '{col.gameObject.tag}'");

        if (_impactoRegistrado) return;

        if (col.gameObject.CompareTag("Ground"))
        {
            Debug.Log("[Bala] Es el suelo, ignorando.");
            return;
        }

        _impactoRegistrado = true;

        float   tiempoVuelo  = Time.time - _tiempoInicio;
        Vector3 puntoImpacto = col.GetContact(0).point;

        // col.impulse requiere ProvidesContacts en Unity 6.
        // Si devuelve 0, se aproxima como J = m * Δv
        float impulso = col.impulse.magnitude;
        if (impulso == 0f)
            impulso = col.relativeVelocity.magnitude * masaDisparo;

        Debug.Log($"[Bala] IMPACTO registrado | Vuelo: {tiempoVuelo:F2}s | Impulso: {impulso:F2}");
        Debug.Log($"[Bala] ReporteTiro.Instancia es null: {ReporteTiro.Instancia == null}");

        if (ReporteTiro.Instancia != null)
            ReporteTiro.Instancia.MostrarReporte(tiempoVuelo, puntoImpacto, impulso);

        Destroy(gameObject, 0.4f);
    }

    void OnDestroy()
    {
        if (!_impactoRegistrado)
        {
            Debug.Log("[Bala] Destruida SIN impacto — enviando reporte de fallo.");
            if (ReporteTiro.Instancia != null)
                ReporteTiro.Instancia.MostrarReporte(Time.time - _tiempoInicio, transform.position, 0f);
        }
    }
}
