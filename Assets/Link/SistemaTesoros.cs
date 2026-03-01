using UnityEngine;
using TMPro; 
using System.Collections.Generic;

public class SistemaTesoros : MonoBehaviour
{
    public int tesorosActuales = 0;
    public int tesorosNecesarios = 3;

    [Header("Interfaz (UI)")]
    public TextMeshProUGUI textoContador; 

    // Lista de memoria para saber cuáles tenemos que devolver si nos pillan
    private List<Tesoro> tesorosGuardados = new List<Tesoro>();

    void Start()
    {
        ActualizarUI();
    }

    public void RecogerTesoro(Tesoro tesoroRecogido)
    {
        tesorosActuales++;
        tesorosGuardados.Add(tesoroRecogido);
        ActualizarUI();
    }

    // Esta función la llamará el guardia cuando te atrape
    public void PerderTesoros()
    {
        tesorosActuales = 0;
        
        // Revivimos cada tesoro que llevábamos encima
        foreach (Tesoro t in tesorosGuardados)
        {
            t.Resetear();
        }
        
        tesorosGuardados.Clear(); 
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoContador != null)
        {
            textoContador.text = tesorosActuales.ToString();
        }
    }
}