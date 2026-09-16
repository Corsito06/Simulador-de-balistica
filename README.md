# Simulador Balístico

Simulador de física de proyectiles desarrollado en Unity. El jugador controla el ángulo de elevación, la rotación horizontal, la fuerza de disparo y la masa del proyectil para derribar una pared de bloques conectados con FixedJoints.

---

## Version de Unity

**Unity 6** (6000.x LTS)

---

## Como jugar

1. Abrir la escena Assets/Scenes/SampleScene.unity.
2. Presionar **Play**.
3. Usar los controles en pantalla:
   - **Slider de Giro (Yaw)** — Apunta el cañón horizontalmente (-90° a 90°).
   - **Slider de Ángulo (Pitch)** — Eleva el cañón (0° a 85°).
   - **Slider de Fuerza** — Velocidad inicial del proyectil (10 a 100 m/s).
   - **Dropdown de Masa** — Seleccionar la masa del proyectil: 0,5 / 1 / 5 / 10 kg.
4. Presionar el botón **DISPARAR**.
5. Al impactar, aparece el **Reporte de Tiro** con todos los datos físicos y la puntuación.
6. Presionar **SIGUIENTE TIRO** para continuar (la estructura NO se resetea).

---

## Controles

| Control | Acción |
|---|---|
| Slider Giro | Rotación horizontal del cañón |
| Slider Ángulo | Elevación del cañón |
| Slider Fuerza | Velocidad inicial del proyectil |
| Dropdown Masa | Masa del proyectil (0.5 / 1 / 5 / 10 kg) |
| Botón DISPARAR | Lanza el proyectil |
| Botón SIGUIENTE TIRO | Cierra el panel de resultados |

---

## Estructura del proyecto

`
Assets/
├── Prefabs/
│   └── Bala.prefab          — Proyectil (Sphere + Rigidbody + SphereCollider + Bala.cs)
├── Scripts/
│   ├── Arma.cs              — CannonController: UI de controles y lanzamiento
│   ├── Bala.cs              — Detecta colisión, registra datos físicos del impacto
│   ├── EstructuraObjetivo.cs — Componente por bloque: detecta si fue derribado
│   ├── ObjetivoPrincipal.cs — Gestor de la estructura: cuenta piezas derribadas
│   └── RegistroDisparo.cs   — Singleton: registra disparos, calcula puntuación, muestra UI
└── Scenes/
    └── SampleScene.unity    — Escena principal
`

---

## Criterios de evaluación

| Criterio | Descripción |
|---|---|
| Física de proyectil | Trayectoria parabólica correcta por gravedad y velocidad inicial |
| Estructura con Joints | Pared de cubos con FixedJoints que se rompen al impacto |
| Registro de impacto | Tiempo de vuelo, punto de impacto, velocidad relativa, impulso, piezas derribadas |
| Puntuación | score = (velocidadRelativa × impulso) + (piezasDerribadas × 50) |
| UI completa | Sliders, dropdown de masa, reporte de tiro en pantalla |
| Estabilidad de la escena | La estructura NO colapsa sola al iniciar Play Mode |

---

## Configuración de la escena (guía rápida)

1. **Suelo (Plane):** asignar Tag Ground.
2. **Cañón (Cylinder):** tiene el script CannonController + campo irePoint (Transform hijo en la boca del cañón).
3. **Bala.prefab:** agregar componente Rigidbody + script Bala + tag Bala.
4. **Estructura objetivo:**
   - Crear un GameObject vacío EstructuraPared con script ObjetivoPrincipal.
   - Añadir hijos de tipo Cube, cada uno con Rigidbody + EstructuraObjetivo + FixedJoint.
   - El cubo base debe tener Rigidbody con IsKinematic = true.
5. **RegistroDisparo:** GameObject vacío con el script RegistroDisparo + referencias a los paneles de UI.

---

## Video de demostración

🎥 [Ver en YouTube](https://www.youtube.com/LINK_PENDIENTE)

*(Link disponible próximamente — ver README actualizado en el repositorio)*

---

## Instalación

`ash
git clone https://github.com/usuario/simulador-balistica.git
`

Abrir con **Unity Hub** → **Open Project** → seleccionar la carpeta raíz.

---

## Autor

Proyecto de simulación balística — 2026
