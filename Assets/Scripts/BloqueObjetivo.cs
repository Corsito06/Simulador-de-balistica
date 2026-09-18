using UnityEngine;

/// <summary>
/// Adjuntar a cada cubo de la pared objetivo.
/// Detecta cuando el cubo toca el suelo (Tag "Suelo") y lo notifica al ReporteTiro.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BloqueObjetivo : MonoBehaviour
{
    private bool _derribado = false;

    void OnCollisionEnter(Collision col)
    {
        if (_derribado) return;

        if (col.gameObject.CompareTag("Suelo"))
        {
            _derribado = true;

            if (ReporteTiro.Instancia != null)
                ReporteTiro.Instancia.RegistrarBloqueDerribado();
        }
    }
}
