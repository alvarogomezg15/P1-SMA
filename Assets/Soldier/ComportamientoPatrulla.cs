using UnityEngine;
using UnityEngine.AI;

public class ComportamientoPatrulla : ComportamientoGuardia
{
    [Header("Rutas Asignadas")]
    public RutaPatrulla rutaPrincipal; 
    public RutaPatrulla[] rutasAlternativas; 
    
    [HideInInspector] public bool haciendoBarrido = false; 

    private RutaPatrulla rutaActual;
    private int indiceDestino = 0;
    private float tiempoParaRecalcular = 0f;

    public void IniciarBarrido()
    {
        Debug.Log("Haciendo barrido de patrulla cercana");
        haciendoBarrido = true;
        cerebro.CambiarComportamiento(this);
    }

    public override void Entrar()
    {
        tiempoParaRecalcular = 1f;
        AsignarMejorRuta();
    }

    public override void Salir()
    {
        if (rutaActual != null && rutaActual != rutaPrincipal)
        {
            rutaActual.Liberar(cerebro);
        }
    }

    public override void Ejecutar()
    {
        // Para ver si falta el tesoro
        if (cerebro.sensores.DescubreRobo() && cerebro.modAlarma != null && cerebro.modAlarma.trampa != null && !cerebro.modAlarma.trampa.estaActivada && !cerebro.modAlarma.yaCerroPuertas)
        {
            cerebro.CambiarComportamiento(cerebro.modAlarma);
            return;
        }

        // Sensores
        if (cerebro.sensores.VeAlJugador())
        {
            haciendoBarrido = false; 
            cerebro.CambiarComportamiento(cerebro.modSorpresa);
            return;
        }
        if (cerebro.sensores.EscuchaAlJugador(out Vector3 puntoEstimado))
        {
            haciendoBarrido = false; 
            cerebro.sensores.ultimaPosicionConocida = puntoEstimado;
            cerebro.CambiarComportamiento(cerebro.modSorpresa);
            return;
        }

        // Espera si no hay rutas disponibles
        if (rutaActual == null || rutaActual.puntos.Length == 0) 
        {
            tiempoParaRecalcular += Time.deltaTime;
            if (tiempoParaRecalcular > 1f)
            {
                tiempoParaRecalcular = 0f;
                AsignarMejorRuta(); 
            }
            return; 
        }

        // Reflejos ante bloqueos
        if (cerebro.motor.TieneCaminoCortado())
        {
            Debug.Log("Paso cortado, recalculando");
            cerebro.motor.Frenar();
            AsignarMejorRuta(forzarAlternativa: true); 
            return;
        }

        // Caminar hacia destino
        tiempoParaRecalcular += Time.deltaTime;
        if (tiempoParaRecalcular > 0.5f)
        {
            tiempoParaRecalcular = 0f;
            cerebro.motor.MoverA(rutaActual.puntos[indiceDestino].position, false);
        }

        // Avanzar al siguiente punto
        if (cerebro.motor.HaLlegado(0.5f))
        {
            indiceDestino++;
            
            if (indiceDestino >= rutaActual.puntos.Length)
            {
                indiceDestino = 0;
                
                if (haciendoBarrido)
                {
                    Debug.Log("Barrido hecho, volviendo a la ruta principal");
                    haciendoBarrido = false;
                }
                
                if (rutaActual != rutaPrincipal && rutaPrincipal != null)
                {
                    AsignarMejorRuta(); 
                }
            }
            tiempoParaRecalcular = 1f; 
        }
    }

    private void AsignarMejorRuta(bool forzarAlternativa = false)
    {
        bool principalAbierta = false;

        if (rutaPrincipal != null && !(forzarAlternativa && rutaActual == rutaPrincipal))
        {
            NavMeshPath rutaTeorica = new NavMeshPath();
            cerebro.motor.GetComponent<NavMeshAgent>().CalculatePath(rutaPrincipal.puntos[0].position, rutaTeorica);
            if (rutaTeorica.status == NavMeshPathStatus.PathComplete)
            {
                principalAbierta = true;
            }
        }

        RutaPatrulla mejorAlternativa = null;
        float menorDistancia = float.MaxValue;

        foreach (RutaPatrulla ruta in rutasAlternativas)
        {
            if (forzarAlternativa && ruta == rutaActual) continue;

            if (ruta != null && (ruta.EstaLibre() || ruta.ocupanteActual == cerebro))
            {
                float distAEstaRuta = float.MaxValue;
                foreach (Transform punto in ruta.puntos)
                {
                    float d = Vector3.Distance(transform.position, punto.position);
                    if (d < distAEstaRuta) distAEstaRuta = d;
                }

                NavMeshPath rutaTeorica = new NavMeshPath();
                cerebro.motor.GetComponent<NavMeshAgent>().CalculatePath(ruta.puntos[0].position, rutaTeorica);
                
                if (rutaTeorica.status == NavMeshPathStatus.PathComplete && distAEstaRuta < menorDistancia)
                {
                    menorDistancia = distAEstaRuta;
                    mejorAlternativa = ruta;
                }
            }
        }

        if (haciendoBarrido)
        {
            if (mejorAlternativa != null)
            {
                CambiarARuta(mejorAlternativa);
                return;
            }
            else
            {
                Debug.Log("Barrido frustrado por bloqueos. Abandono la intención.");
                haciendoBarrido = false;
            }
        }

        if (principalAbierta && !forzarAlternativa)
        {
            CambiarARuta(rutaPrincipal);
            return;
        }

        if (mejorAlternativa != null)
        {
            CambiarARuta(mejorAlternativa);
        }
        else if (principalAbierta)
        {
            CambiarARuta(rutaPrincipal);
        }
        else
        {
            CambiarARuta(null);
            cerebro.motor.Frenar();
        }
    }

    private void CambiarARuta(RutaPatrulla nuevaRuta)
    {
        if (rutaActual == nuevaRuta) return;

        if (rutaActual != null && rutaActual != rutaPrincipal)
            rutaActual.Liberar(cerebro);

        rutaActual = nuevaRuta;
        indiceDestino = 0;

        if (rutaActual != null && rutaActual != rutaPrincipal)
            rutaActual.Reclamar(cerebro);
    }
}