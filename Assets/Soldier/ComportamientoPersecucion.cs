using UnityEngine;

public class ComportamientoPersecucion : ComportamientoGuardia
{
    public override void Entrar()
    {
        Debug.Log("Link encontrado");
    }

    public override void Ejecutar()
    {
        // Condicion de victoria guardia
        if (cerebro.sensores.DistanciaAlJugador() < 1.5f)
        {
            Debug.Log("Link atrapado");
            
            // Quitar tesoros
            SistemaTesoros inventarioLink = cerebro.sensores.objetivo.GetComponent<SistemaTesoros>();
            if (inventarioLink != null)
            {
                inventarioLink.PerderTesoros();
            }
            
            cerebro.sensores.objetivo.position = cerebro.destinoLobby.position;
            cerebro.sensores.objetivo.rotation = cerebro.destinoLobby.rotation;
            
            cerebro.CambiarComportamiento(cerebro.modPatrulla);
            return;
        }


        // Logica persecucion
        if (cerebro.sensores.VeAlJugador())
        {
            // Mientras ve a link actualiza su destino y corre
            cerebro.motor.MoverA(cerebro.sensores.objetivo.position, true);
        }
        else
        {
            // Si se esconde tras un muro, el módulo aborta y llama a Búsqueda
            cerebro.CambiarComportamiento(cerebro.modBusqueda);
        }
    }
}