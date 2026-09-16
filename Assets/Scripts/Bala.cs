using UnityEngine;

/// <summary>
/// Componente de la bala. Registra tiempo de vuelo y datos de impacto,
/// luego los envía al RegistroDisparo.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bala : MonoBehaviour
{
    // Datos del disparo configurados por CannonController al instanciar
    [HideInInspector] public float anguloDisparo;
    [HideInInspector] public float fuerzaDisparo;
    [HideInInspector] public float masaDisparo;
    [HideInInspector] public int   numeroDisparo;

    private float     _tiempoInicio;
    private bool      _impactoRegistrado = false;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _tiempoInicio = Time.time;
    }

    void OnCollisionEnter(Collision col)
    {
        if (_impactoRegistrado) return;

        // Ignorar colision con el suelo (tagear el Plane como "Ground" en el Inspector)
        if (col.gameObject.CompareTag("Ground")) return;

        _impactoRegistrado = true;

        float   tiempoVuelo  = Time.time - _tiempoInicio;
        Vector3 puntoImpacto = col.contacts[0].point;
        float   velRelativa  = col.relativeVelocity.magnitude;
        float   impulso      = col.impulse.magnitude;

        if (RegistroDisparo.Instancia != null)
        {
            RegistroDisparo.Instancia.RegistrarImpacto(
                numeroDisparo, tiempoVuelo, puntoImpacto,
                velRelativa, impulso,
                anguloDisparo, fuerzaDisparo, masaDisparo,
                col.gameObject.name
            );
        }

        // Destruir la bala un poco despues para que el impulso se aplique
        Destroy(gameObject, 0.1f);
    }

    // Si la bala nunca impacta (cae fuera de rango), reportar de todas formas
    void OnDestroy()
    {
        if (!_impactoRegistrado && RegistroDisparo.Instancia != null)
        {
            float tiempoVuelo = Time.time - _tiempoInicio;
            RegistroDisparo.Instancia.RegistrarImpacto(
                numeroDisparo, tiempoVuelo, transform.position,
                0f, 0f,
                anguloDisparo, fuerzaDisparo, masaDisparo,
                "Sin impacto"
            );
        }
    }
}
