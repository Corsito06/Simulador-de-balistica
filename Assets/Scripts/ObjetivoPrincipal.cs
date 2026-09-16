using System.Collections.Generic;
using UnityEngine;

public class ObjetivoPrincipal : MonoBehaviour
{
    [Header("Piezas de la estructura")]
    public List<EstructuraObjetivo> piezas = new List<EstructuraObjetivo>();

    private int _piezasDerribadasEsteTiro = 0;
    public int TotalDerribadas { get; private set; } = 0;
    public int TotalPiezas => piezas.Count;

    void Awake()
    {
        if (piezas.Count == 0)
            piezas.AddRange(GetComponentsInChildren<EstructuraObjetivo>());

        foreach (var p in piezas)
            if (p.gestor == null) p.gestor = this;
    }

    public void IniciarConteoTiro()
    {
        _piezasDerribadasEsteTiro = 0;
    }

    public void NotificarPiezaDerribada(EstructuraObjetivo pieza)
    {
        _piezasDerribadasEsteTiro++;
        TotalDerribadas++;
    }

    public int PiezasDerribadasEsteTiro => _piezasDerribadasEsteTiro;

    public int PiezasEnPie
    {
        get
        {
            int enPie = 0;
            foreach (var p in piezas)
                if (!p.EstaDerribada) enPie++;
            return enPie;
        }
    }
}
