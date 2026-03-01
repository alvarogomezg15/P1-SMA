using UnityEngine;

public class ComportamientoBusqueda : ComportamientoGuardia
{
    [Header("Configuración")]
    public float tiempoMaximoBusqueda = 3f;
    private float temporizador = 0f;
    private bool buscandoEnSitio = false;

    public override void Entrar()
    {
        Debug.Log("Link perdido de vista, buscándolo...");
        temporizador = 0f;
        buscandoEnSitio = false;
        
        // Vuelve a ir a donde vio a Link por ultima vez
        cerebro.motor.MoverA(cerebro.sensores.ultimaPosicionConocida, true);
    }

    public override void Ejecutar()
    {
        if (cerebro.sensores.VeAlJugador())
        {
            cerebro.CambiarComportamiento(cerebro.modSorpresa);
            return;
        }

        
        if (cerebro.motor.HaLlegado(1.5f) || cerebro.motor.TieneCaminoCortado())
        {
            if (!buscandoEnSitio)
            {
                buscandoEnSitio = true;
                cerebro.motor.Frenar(); // Se para
            }

            temporizador += Time.deltaTime;
            if (temporizador >= tiempoMaximoBusqueda)
            {
                Debug.Log("Link perdido completamente");
                
                cerebro.modPatrulla.IniciarBarrido(); 
            }
        }
    }
}