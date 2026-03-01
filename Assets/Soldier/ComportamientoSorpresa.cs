using UnityEngine;

public class ComportamientoSorpresa : ComportamientoGuardia
{
    [Header("Configuración")]
    public float tiempoDeReaccion = 0.6f;
    private float temporizador = 0f;

    public override void Entrar()
    {
        temporizador = 0f;
        cerebro.motor.Frenar();
        Debug.Log("????");
    }

    public override void Ejecutar()
    {
        // Contamos el tiempo
        temporizador += Time.deltaTime;
        
        // Cuando pasa el susto, le decimos al cerebro que cambie a Persecución
        if (temporizador >= tiempoDeReaccion)
        {
            cerebro.CambiarComportamiento(cerebro.modPersecucion);
        }
    }
}