using UnityEngine;

public class TeletransportadorSimple : MonoBehaviour
{
    [Header("¿A dónde te lleva?")]
    public Transform destino; // Arrastraremos aquí el punto vacío donde quieres aparecer

    [Header("Condición de Victoria")]
    [Tooltip("Si marcas esto, Link necesitará los 3 tesoros para poder usarlo.")]
    public bool requiereTesoros = false; 

    // Esta función salta sola cuando algo entra en el círculo
    void OnTriggerEnter(Collider other)
    {
        // 1. Comprobamos si lo que ha entrado es Link
        CerebroLink link = other.GetComponent<CerebroLink>();

        if (link != null)
        {
            // 2. Si este teletransportador es la SALIDA, pedimos los tesoros
            if (requiereTesoros)
            {
                SistemaTesoros inventario = other.GetComponent<SistemaTesoros>();
                
                // Comprobamos si tiene el inventario y si ha llegado a 3
                if (inventario != null && inventario.tesorosActuales >= inventario.tesorosNecesarios)
                {
                    Debug.Log("🏆 ¡HAS GANADO! Has robado los 3 tesoros y has escapado.");
                    EjecutarTeletransporte(other);
                }
                else
                {
                    // Si no los tiene, no hace nada físico, solo avisa.
                    Debug.Log("❌ Aún te faltan tesoros para escapar. Vuelve cuando tengas los 3.");
                }
            }
            // 3. Si NO requiere tesoros (ej: del Lobby al Mapa), teletransporta directo
            else 
            {
                EjecutarTeletransporte(other);
            }
        }
    }

    // He sacado el teletransporte a una función aparte para no repetir código
    private void EjecutarTeletransporte(Collider other)
    {
        other.transform.position = destino.position;
        other.transform.rotation = destino.rotation; 

        Debug.Log("🚀 ¡Teletransporte completado!");
    }
}