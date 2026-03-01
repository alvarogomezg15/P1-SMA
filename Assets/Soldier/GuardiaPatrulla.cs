using UnityEngine;
using UnityEngine.AI;

public class GuardiaPatrulla : MonoBehaviour
{
    
    public enum EstadoGuardia { Patrullando, Sorprendido, Persiguiendo, Buscando }
    public EstadoGuardia estadoActual = EstadoGuardia.Patrullando;

    [Header("Velocidades")]
    public float velocidadBaja = 2.5f; // Patrullando
    public float velocidadAlta = 4.5f; // Persiguiendo a Link

    [Header("Ruta de Patrulla")]
    public Transform[] puntosDeRuta; 
    private int indiceDestino = 0; 
    private NavMeshAgent agente;  
    private float tiempoParaRecalcular = 0f; 

    [Header("Sensores de Visión")]
    public Transform objetivo; 
    public float radioVision = 12f; 
    public float anguloVision = 45f; 
    
    [Header("Reacción y Búsqueda")]
    public float tiempoDeReaccion = 0.6f; // Tiempo que tarda en reaccionar antes de echar a correr
    private float contadorReaccion = 0f;
    private Vector3 ultimaPosicionConocida;
    private float tiempoBuscando = 0f;
    public float tiempoMaximoBusqueda = 3f;

    [Header("Castigo")]
    public Transform destinoLobby; 

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadBaja; // Empieza caminando despacio
        tiempoParaRecalcular = 1f; 
    }

    void Update()
    {
        bool veAlJugador = BuscarAlJugador();

        switch (estadoActual)
        {
            case EstadoGuardia.Patrullando:
                ComportamientoPatrulla(veAlJugador);
                break;
            case EstadoGuardia.Sorprendido:
                ComportamientoSorprendido();
                break;
            case EstadoGuardia.Persiguiendo:
                ComportamientoPersecucion(veAlJugador);
                break;
            case EstadoGuardia.Buscando:
                ComportamientoBusqueda(veAlJugador);
                break;
        }
    }

    bool BuscarAlJugador()
    {
        if (objetivo == null) return false;

        Vector3 direccionHaciaLink = objetivo.position - transform.position;
        float distancia = direccionHaciaLink.magnitude;

        if (distancia < radioVision)
        {
            float anguloALink = Vector3.Angle(transform.forward, direccionHaciaLink);
            if (anguloALink < anguloVision)
            {
                Vector3 origen = transform.position + Vector3.up * 1f;
                Vector3 destino = objetivo.position + Vector3.up * 1f;
                
                RaycastHit impacto;
                if (Physics.Raycast(origen, destino - origen, out impacto, radioVision))
                {
                    if (impacto.collider.CompareTag("Player"))
                    {
                        ultimaPosicionConocida = objetivo.position; 
                        return true; 
                    }
                }
            }
        }
        return false; 
    }

    void ComportamientoPatrulla(bool veAlJugador)
    {
        // Esto es el mítico ! de reaccion
        if (veAlJugador)
        {
            estadoActual = EstadoGuardia.Sorprendido;
            contadorReaccion = 0f; 
            agente.isStopped = true; // Clava los frenos
            Debug.Log("Hostias!");
            return;
        }

        if (puntosDeRuta.Length == 0) return;

        tiempoParaRecalcular += Time.deltaTime;
        if (tiempoParaRecalcular > 0.5f)
        {
            tiempoParaRecalcular = 0f; 
            NavMeshPath ruta = new NavMeshPath();
            agente.CalculatePath(puntosDeRuta[indiceDestino].position, ruta);

            if (ruta.status == NavMeshPathStatus.PathComplete)
            {
                agente.isStopped = false; 
                agente.SetPath(ruta);     
            }
            else 
            {
                agente.ResetPath();      
                agente.isStopped = true; 
            }
        }

        if (!agente.pathPending && !agente.isStopped && agente.remainingDistance < 0.5f)
        {
            indiceDestino = (indiceDestino + 1) % puntosDeRuta.Length;
            tiempoParaRecalcular = 1f; 
        }
    }

    
    void ComportamientoSorprendido()
    {
        contadorReaccion += Time.deltaTime;
        
        // Cuando pasa el susto, arranca a correr
        if (contadorReaccion >= tiempoDeReaccion)
        {
            estadoActual = EstadoGuardia.Persiguiendo;
            agente.speed = velocidadAlta; // Se pone a correr
            Debug.Log("Vení loco");
        }
    }

    void ComportamientoPersecucion(bool veAlJugador)
    {
        float distanciaFisica = Vector3.Distance(transform.position, objetivo.position);
        
        if (distanciaFisica < 0.75f) 
        {
            Debug.Log("Link ha sido pillado");
            
            if (destinoLobby != null)
            {
                objetivo.position = destinoLobby.position;
                objetivo.rotation = destinoLobby.rotation; 
            }

            estadoActual = EstadoGuardia.Patrullando;
            agente.speed = velocidadBaja; // Vuelve a caminar despacio
            tiempoParaRecalcular = 1f; 
            
            return; 
        }

        if (veAlJugador)
        {
            agente.isStopped = false;
            agente.SetDestination(objetivo.position);
        }
        else
        {
            estadoActual = EstadoGuardia.Buscando;
            tiempoBuscando = 0f; 
            agente.SetDestination(ultimaPosicionConocida); 
            Debug.Log("Link perdido");
        }
    }

    void ComportamientoBusqueda(bool veAlJugador)
    {
        if (veAlJugador)
        {
            estadoActual = EstadoGuardia.Sorprendido; // Si asomas, se vuelve a sorprender medio segundo
            contadorReaccion = 0f;
            agente.isStopped = true;
            Debug.Log("Link encontrado de nuevo");
            return;
        }

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            tiempoBuscando += Time.deltaTime;

            if (tiempoBuscando >= tiempoMaximoBusqueda)
            {
                Debug.Log("Link perdido completamente");
                estadoActual = EstadoGuardia.Patrullando;
                agente.speed = velocidadBaja; // Se relaja y vuelve a caminar despacio
                tiempoParaRecalcular = 1f; 
            }
        }
    }
}