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
    [SerializeField] private TMP_Dropdown massDropdown;  // Seleccion de masa
    [SerializeField] private Button      shootButton;

    [Header("UI - Textos de informacion")]
    [SerializeField] private TextMeshProUGUI yawText;
    [SerializeField] private TextMeshProUGUI pitchText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI massText;

    // Valores de masa disponibles (kg)
    private static readonly float[] MasasDisponibles = { 0.5f, 1f, 5f, 10f };

    private float _currentYaw;
    private float _currentPitch;
    private float _currentForce;
    private float _currentMass = 1f;
    private int   _shotNumber  = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // --- Sliders ---
        if (yawSlider   != null) yawSlider.onValueChanged.AddListener(_ => OnYawChanged());
        if (pitchSlider != null) pitchSlider.onValueChanged.AddListener(_ => OnPitchChanged());
        if (powerSlider != null) powerSlider.onValueChanged.AddListener(_ => OnPowerChanged());

        // --- Dropdown de masa ---
        if (massDropdown != null)
        {
            massDropdown.ClearOptions();
            var opts = new System.Collections.Generic.List<string>();
            foreach (float m in MasasDisponibles)
                opts.Add(m + " kg");
            massDropdown.AddOptions(opts);
            massDropdown.value = 1;         // Valor por defecto: 1 kg
            massDropdown.onValueChanged.AddListener(OnMassChanged);
        }

        // --- Boton de disparo ---
        if (shootButton != null)
            shootButton.onClick.AddListener(Shoot);

        // Inicializar lecturas y rotacion inicial
        OnYawChanged();
        OnPitchChanged();
        OnPowerChanged();
        OnMassChanged(massDropdown != null ? massDropdown.value : 1);
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

    void OnMassChanged(int index)
    {
        if (index < 0 || index >= MasasDisponibles.Length) return;
        _currentMass = MasasDisponibles[index];
        if (massText != null) massText.text = $"Masa: {_currentMass} kg";
    }

    void UpdateRotation()
    {
        // 90f mantiene el cilindro acostado horizontalmente
        transform.localRotation = Quaternion.Euler(90f - _currentPitch, _currentYaw, 0f);
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        _shotNumber++;

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
            rb.mass = _currentMass;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = firePoint.forward * _currentForce;
        }

        // Pasar metadatos del disparo a la bala
        Bala balaComp = proj.GetComponent<Bala>();
        if (balaComp != null)
        {
            balaComp.anguloDisparo  = _currentPitch;
            balaComp.fuerzaDisparo  = _currentForce;
            balaComp.masaDisparo    = _currentMass;
            balaComp.numeroDisparo  = _shotNumber;
        }

        // Auto-destruir si no impacta en 8 segundos
        Destroy(proj, 8f);
    }
}
