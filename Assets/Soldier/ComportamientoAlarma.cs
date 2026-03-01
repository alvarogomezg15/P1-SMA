using UnityEngine;

public class ComportamientoAlarma : ComportamientoGuardia
{
    [Header("Misión de Cierre")]
    public Transform puntoCierre; // Un objeto vacío donde el guardia activa la trampa
    public TrampaLobby trampa;
    
    [HideInInspector] public bool yaCerroPuertas = false;    // El script de las vallas

    public override void Entrar()
    {
        Debug.Log("Robaron un tesoro! Voy a cerrar la puerta!");
        // Corre hacia el punto de cierre
        cerebro.motor.MoverA(puntoCierre.position, true); 
    }

    public override void Ejecutar()
    {
        // Si ve a Link por el camino lo persigue
        if (cerebro.sensores.VeAlJugador())
        {
            cerebro.CambiarComportamiento(cerebro.modSorpresa);
            return;
        }

        // Si le ponemos un bloque en medio lo esquiva
        if (cerebro.motor.TieneCaminoCortado())
        {
            cerebro.motor.Frenar();
            cerebro.motor.MoverA(puntoCierre.position, true);
            return;
        }

        // Llega al interruptor
        if (cerebro.motor.HaLlegado(1.5f))
        {
            trampa.ActivarVallas();
            
            // Al bloquear la salida el guardia barre el mapa
            cerebro.modPatrulla.haciendoBarrido = true;
            yaCerroPuertas = true; // Para que recuerde que ya cumplió su misión
            cerebro.CambiarComportamiento(cerebro.modPatrulla);
        }
    }
}