# Simulador Balistico

Simulador de fisica de proyectiles desarrollado en **Unity 6**. El jugador controla el angulo de elevacion, la rotacion horizontal, la fuerza de disparo y la masa del proyectil para derribar una pared de bloques. El sistema de fisicas de Unity gobierna la trayectoria y las colisiones. Al impactar, se registran los datos del disparo y se muestra un reporte completo en pantalla.

---

## Version de Unity

**Unity 6** (6000.x LTS)

---

## Como jugar

1. Abrir la escena Assets/Scenes/SampleScene.unity.
2. Presionar **Play**.
3. Configurar el disparo con los controles en pantalla:
   - **Slider de Giro (Yaw)** — Apunta el canon horizontalmente (-90 a 90 grados).
   - **Slider de Angulo (Pitch)** — Eleva el canon (0 a 85 grados).
   - **Slider de Fuerza** — Velocidad inicial del proyectil (m/s).
   - **Slider de Masa** — Masa del proyectil en kg.
4. **Clic izquierdo del mouse** en la escena para disparar (no dispara si el cursor esta sobre la UI).
5. Observar la trayectoria parabolica del proyectil.
6. Al impactar, esperar 1.5 segundos y aparece el **Panel de Reporte** con los datos del tiro.
7. Presionar **Cerrar** para continuar disparando (la estructura NO se resetea entre tiros).

---

## Controles

| Control | Accion |
|---|---|
| Slider Giro | Rotacion horizontal del canon |
| Slider Angulo | Elevacion del canon |
| Slider Fuerza | Velocidad inicial del proyectil |
| Slider Masa | Masa del proyectil (kg) |
| Clic izquierdo (fuera de la UI) | Disparar |
| Boton Cerrar (panel) | Cierra el reporte y habilita el siguiente tiro |

---

## Datos del Reporte de Tiro

Tras cada impacto, el panel muestra:

| Dato | Descripcion |
|---|---|
| **Tiempo de vuelo** | Segundos desde el disparo hasta el primer impacto |
| **Punto de impacto** | Coordenadas XYZ del contacto (via collision.GetContact(0).point) |
| **Impulso** | Magnitud del impulso fisico aplicado en la colision (N·s) |
| **Bloques derribados** | Cantidad de cubos que se desplazaron de su posicion original |

---

## Estructura del proyecto

`
Assets/
+-- Prefabs/
|   +-- Bala.prefab          Proyectil (Sphere + Rigidbody + SphereCollider + Bala.cs)
+-- Scripts/
|   +-- Arma.cs              CannonController: sliders de UI, rotacion del canon, disparo
|   +-- Bala.cs              Cronometra el vuelo, detecta impacto, envia datos al ReporteTiro
|   +-- ReporteTiro.cs       Singleton: recibe datos, cuenta bloques, activa el panel de UI
|   +-- BloqueObjetivo.cs    Componente por cubo: detecta si fue derribado (joint, posicion o suelo)
+-- Scenes/
    +-- SampleScene.unity    Escena principal
`

---

## Criterios de evaluacion

| Criterio | Implementacion |
|---|---|
| Fisica de proyectil | Rigidbody + b.linearVelocity segun angulo. Trayectoria parabolica por gravedad |
| Control de parametros | Sliders de Yaw, Pitch, Fuerza y Masa conectados al CannonController |
| Disparo por input | Input.GetMouseButtonDown(0) con guarda de EventSystem para no disparar sobre la UI |
| Registro de impacto | Tiempo de vuelo, punto de impacto via GetContact(0).point, impulso via collision.impulse |
| Conteo de bloques | Triple deteccion: OnJointBreak, desplazamiento > 0.3m, colision con suelo |
| UI de reporte | Panel de Canvas activado por corrutina 1.5s despues del impacto (TextMeshPro) |
| Camara secundaria | Render Texture + Raw Image para vista alternativa del muro |
| Estabilidad inicial | La estructura NO colapsa sola. NO se resetea entre disparos |

---

## Configuracion de la escena (referencia rapida)

1. **Suelo (Plane):** Tag = Ground.
2. **Canon (Cylinder):** script Arma.cs + Transform hijo irePoint en la boca del canon.
3. **Bala.prefab:** componentes Rigidbody + SphereCollider + script Bala + Tag = Bala.
4. **Estructura objetivo:** cubos con Rigidbody + script BloqueObjetivo (+ FixedJoint opcional).
5. **ReporteTiro:** GameObject vacio con script ReporteTiro + referencias al panel, textos y boton en el Inspector.
6. **Camara secundaria:** Camera con Target Texture = Render Texture + Raw Image en Canvas con la misma Render Texture.

---

## Video de demostracion

Video en YouTube: [Ver demo](https://www.youtube.com/LINK_PENDIENTE)

*(Reemplazar el link una vez subido el video)*

---

## Instalacion

`ash
git clone https://github.com/usuario/simulador-balistica.git
`

Abrir con **Unity Hub** → **Open Project** → seleccionar la carpeta raiz.

---

## Autor

Proyecto de simulacion balistica — 2026
