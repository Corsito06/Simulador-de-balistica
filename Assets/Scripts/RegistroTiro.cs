using System;
using System.Collections.Generic;

/// <summary>
/// Modelo de datos de un tiro. Compatible con JsonUtility para Cloud Save.
/// </summary>
[Serializable]
public class RegistroTiro
{
    public int    numero;
    public float  angulo;           // grados de elevacion (pitch)
    public float  fuerza;           // velocidad inicial m/s
    public float  masa;             // kg
    public float  tiempoVuelo;      // segundos hasta el impacto
    public float  impulso;          // N*s
    public int    bloquesDerribados;
    public bool   acierto;          // true si impacto > 0
    public float  distanciaImpacto; // distancia entre disparo e impacto
    public string timestamp;        // fecha y hora del tiro
}

/// <summary>
/// Lista completa de tiros — se serializa como JSON para Cloud Save.
/// </summary>
[Serializable]
public class HistorialDisparos
{
    public List<RegistroTiro> disparos = new List<RegistroTiro>();
}
