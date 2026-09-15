using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para TextMeshPro

public class CannonController : MonoBehaviour
{
    [Header("Referencias del Cañón")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("UI - Controles")]
    [SerializeField] private Slider yawSlider;     // Giro horizontal (-90 a 90)
    [SerializeField] private Slider pitchSlider;   // Ángulo vertical (0 a 85)
    [SerializeField] private Slider powerSlider;   // Fuerza / Velocidad inicial
    [SerializeField] private Button shootButton;

    [Header("UI - Textos de Información")]
    [SerializeField] private TextMeshProUGUI yawText;
    [SerializeField] private TextMeshProUGUI pitchText;
    [SerializeField] private TextMeshProUGUI powerText;

    private float currentYaw;
    private float currentPitch;
    private float currentForce;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Escuchar cambios de los Sliders
        if (yawSlider != null)
        {
            yawSlider.onValueChanged.AddListener(delegate { OnYawChanged(); });
        }

        if (pitchSlider != null)
        {
            pitchSlider.onValueChanged.AddListener(delegate { OnPitchChanged(); });
        }

        if (powerSlider != null)
        {
            powerSlider.onValueChanged.AddListener(delegate { OnPowerChanged(); });
        }

        // Asignar botón de disparo
        if (shootButton != null)
        {
            shootButton.onClick.AddListener(Shoot);
        }

        // Inicializar lecturas y orientación
        OnYawChanged();
        OnPitchChanged();
        OnPowerChanged();
    }

    void OnYawChanged()
    {
        if (yawSlider == null) return;
        currentYaw = yawSlider.value;
        if (yawText != null) yawText.text = $"Giro: {currentYaw:F1}°";
        UpdateRotation();
    }

    void OnPitchChanged()
    {
        if (pitchSlider == null) return;
        currentPitch = pitchSlider.value;
        if (pitchText != null) pitchText.text = $"Ángulo: {currentPitch:F1}°";
        UpdateRotation();
    }

    void OnPowerChanged()
    {
        if (powerSlider == null) return;
        currentForce = powerSlider.value;
        if (powerText != null) powerText.text = $"Fuerza: {currentForce:F1} m/s";
    }

    void UpdateRotation()
    {
        // 90f mantiene el cilindro acostado
        transform.localRotation = Quaternion.Euler(90f - currentPitch, currentYaw, 0f);
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = firePoint.forward * currentForce;
        }

        Destroy(projectile, 6f);
    }
}