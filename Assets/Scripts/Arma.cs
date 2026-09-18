using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CannonController : MonoBehaviour
{
    [Header("Referencias del Canon")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;

    [Header("UI - Controles de disparo")]
    [SerializeField] private Slider      yawSlider;      // Giro horizontal (-90 a 90)
    [SerializeField] private Slider      pitchSlider;    // Angulo vertical (0 a 85)
    [SerializeField] private Slider      powerSlider;    // Fuerza / Velocidad inicial
    public Slider massSlider;                             // Masa del proyectil (kg)

    [Header("UI - Textos de informacion")]
    [SerializeField] private TextMeshProUGUI yawText;
    [SerializeField] private TextMeshProUGUI pitchText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI massText;

    private float _currentYaw;
    private float _currentPitch;
    private float _currentForce;
    private int   _shotNumber = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // --- Sliders ---
        if (yawSlider   != null) yawSlider.onValueChanged.AddListener(_ => OnYawChanged());
        if (pitchSlider != null) pitchSlider.onValueChanged.AddListener(_ => OnPitchChanged());
        if (powerSlider != null) powerSlider.onValueChanged.AddListener(_ => OnPowerChanged());

        // --- Slider de masa ---
        if (massSlider != null)
        {
            massSlider.onValueChanged.AddListener(_ => OnMassChanged());
            OnMassChanged(); // mostrar valor inicial
        }

        // Inicializar lecturas y rotacion inicial
        OnYawChanged();
        OnPitchChanged();
        OnPowerChanged();
    }

    void OnYawChanged()
    {
        if (yawSlider == null) return;
        _currentYaw = yawSlider.value;
        if (yawText != null) yawText.text = $"Giro: {_currentYaw:F1}°";
        UpdateRotation();
    }

    void OnPitchChanged()
    {
        if (pitchSlider == null) return;
        _currentPitch = pitchSlider.value;
        if (pitchText != null) pitchText.text = $"Angulo: {_currentPitch:F1}°";
        UpdateRotation();
    }

    void OnPowerChanged()
    {
        if (powerSlider == null) return;
        _currentForce = powerSlider.value;
        if (powerText != null) powerText.text = $"Fuerza: {_currentForce:F1} m/s";
    }

    void OnMassChanged()
    {
        if (massSlider == null) return;
        if (massText != null) massText.text = $"Masa: {massSlider.value:F1} kg";
    }

    void UpdateRotation()
    {
        // 90f mantiene el cilindro acostado horizontalmente
        transform.localRotation = Quaternion.Euler(90f - _currentPitch, _currentYaw, 0f);
    }

    void Update()
    {
        // Clic izquierdo del mouse dispara el proyectil.
        // La guarda de EventSystem evita disparar al hacer clic en sliders o dropdowns de la UI.
        if (Input.GetMouseButtonDown(0) &&
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            Shoot();
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        _shotNumber++;

        // Leer masa actual directo del slider
        float masa = (massSlider != null) ? massSlider.value : 1f;

        // Notificar al registro que empieza un nuevo tiro
        if (RegistroDisparo.Instancia != null)
            RegistroDisparo.Instancia.IniciarTiro(_shotNumber);

        // Instanciar la bala
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.tag = "Bala";

        // Configurar Rigidbody
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = massSlider.value;                                  // masa exacta del slider
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = firePoint.forward * _currentForce;       // aplicar fuerza
        }

        // Pasar metadatos del disparo a la bala
        Bala balaComp = proj.GetComponent<Bala>();
        if (balaComp != null)
        {
            balaComp.anguloDisparo  = _currentPitch;
            balaComp.fuerzaDisparo  = _currentForce;
            balaComp.masaDisparo    = masa;
            balaComp.numeroDisparo  = _shotNumber;
        }

        // Auto-destruir si no impacta en 8 segundos
        Destroy(proj, 8f);
    }
}
