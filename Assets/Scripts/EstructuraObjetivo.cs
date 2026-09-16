using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EstructuraObjetivo : MonoBehaviour
{
    public ObjetivoPrincipal gestor;
    [SerializeField] private float umbralVelocidad = 0.8f;
    private bool _derribada = false;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (gestor == null)
            gestor = GetComponentInParent<ObjetivoPrincipal>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (_derribada) return;
        if (!col.gameObject.CompareTag("Bala")) return;
        if (col.impulse.magnitude > 0.5f) MarcarDerribada();
    }

    void FixedUpdate()
    {
        if (_derribada || _rb == null) return;
        if (_rb.angularVelocity.magnitude > umbralVelocidad) MarcarDerribada();
    }

    private void MarcarDerribada()
    {
        _derribada = true;
        FixedJoint fj = GetComponent<FixedJoint>();
        if (fj != null) Destroy(fj);
        if (gestor != null) gestor.NotificarPiezaDerribada(this);
    }

    public bool EstaDerribada => _derribada;
    public void Resetear() { _derribada = false; }
}
