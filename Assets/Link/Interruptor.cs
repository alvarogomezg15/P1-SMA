using UnityEngine;

public class Interruptor : MonoBehaviour
{
    [Header("Conexión con el Main Hall")]
    public TrampaLobby trampa; // Arrastraremos aquí el Gestor_Vallas

    private bool jugadorCerca = false;

    void Update()
    {
        // Si Link está en la zona, las vallas están encendidas, y pulsa la tecla 'E'
        if (jugadorCerca && trampa.estaActivada && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Link ha desactivado las vallas");
            trampa.DesactivarVallas();
            
            // Aquí en el futuro podrías añadir un sonido de "Puerta Abriendo"
        }
    }

    // Link entra en la zona del botón
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (trampa.estaActivada)
            {
                Debug.Log("Pulsa 'E' para abrir las compuertas.");
            }
        }
    }

    // Link sale de la zona del botón
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}