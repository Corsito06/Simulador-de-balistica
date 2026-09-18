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
    }

    void OnCollisionEnter(Collision col)
    {
        if (_impactoRegistrado) return;

        // Ignorar el suelo: la bala puede rebotar en el piso sin disparar el reporte
        if (col.gameObject.CompareTag("Suelo")) return;

        _impactoRegistrado = true;

        float   tiempoVuelo  = Time.time - _tiempoInicio;
        Vector3 puntoImpacto = col.GetContact(0).point;   // API recomendada por Unity
        float   impulso      = col.impulse.magnitude;

        if (ReporteTiro.Instancia != null)
            ReporteTiro.Instancia.MostrarReporte(tiempoVuelo, puntoImpacto, impulso);

        // Destruir la bala un frame despues para que el impulso fisico se aplique
        Destroy(gameObject, 0.05f);
    }

    // Fallback: si la bala cae fuera de rango y es destruida sin impactar
    void OnDestroy()
    {
        if (!_impactoRegistrado && ReporteTiro.Instancia != null)
        {
            float tiempoVuelo = Time.time - _tiempoInicio;
            ReporteTiro.Instancia.MostrarReporte(tiempoVuelo, transform.position, 0f);
        }
    }
}
