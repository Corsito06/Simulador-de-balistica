using UnityEngine;

/// <summary>
/// Adjuntar a cada cubo de la pared objetivo.
/// Cuenta el bloque como derribado si:
///   1) Su joint se rompe (OnJointBreak)
///   2) Se desplaza mas de 'distanciaMinima' de su posicion original (FixedUpdate)
///   3) Toca el suelo directamente (fallback)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BloqueObjetivo : MonoBehaviour
{
    [Tooltip("Distancia minima desde la posicion original para considerarse derribado.")]
    [SerializeField] private float distanciaMinima = 0.3f;

    private bool    _derribado        = false;
    private Vector3 _posicionInicial;
    private Rigidbody _rb;

    void Start()
    {
        _posicionInicial = transform.position;
        _rb              = GetComponent<Rigidbody>();
    }

    // Metodo 1: el joint del cubo se rompe por el impacto
    void OnJointBreak(float breakForce)
    {
        MarcarDerribado();
    }

    // Metodo 2: el cubo se desplazo suficiente de su posicion inicial
    void FixedUpdate()
    {
        if (_derribado) return;
        if (Vector3.Distance(transform.position, _posicionInicial) > distanciaMinima)
            MarcarDerribado();
    }

    // Metodo 3: fallback — toca directamente el suelo
    void OnCollisionEnter(Collision col)
    {
        if (_derribado) return;
        if (col.gameObject.CompareTag("Ground"))
            MarcarDerribado();
    }

    private void MarcarDerribado()
    {
        if (_derribado) return;
        _derribado = true;

        if (ReporteTiro.Instancia != null)
            ReporteTiro.Instancia.RegistrarBloqueDerribado();
    }
}
