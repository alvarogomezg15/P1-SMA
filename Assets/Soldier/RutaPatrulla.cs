using UnityEngine;

public class RutaPatrulla : MonoBehaviour
{
    [Header("Puntos de esta ruta")]
    public Transform[] puntos;
    
    // Aquí guardamos quién está patrullando esta ruta ahora mismo
    [HideInInspector] public GuardiaCerebro ocupanteActual;

    // Función para que los guardias pregunten si pueden pasar
    public bool EstaLibre()
    {
        return ocupanteActual == null;
    }

    // El guardia reclama la ruta
    public void Reclamar(GuardiaCerebro guardia)
    {
        ocupanteActual = guardia;
    }

    // El guardia suelta la ruta para que otro la use
    public void Liberar(GuardiaCerebro guardia)
    {
        if (ocupanteActual == guardia)
        {
            ocupanteActual = null;
        }
    }
}