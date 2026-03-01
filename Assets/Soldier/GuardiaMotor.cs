using UnityEngine;
using UnityEngine.AI;

public class GuardiaMotor : MonoBehaviour
{
    [Header("Ajustes de Velocidad")]
    public float velocidadBaja = 2.5f;
    public float velocidadAlta = 4.5f;

    private NavMeshAgent agente;
    private Animator animator;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agente.speed = velocidadBaja;
    }

    void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agente.velocity.magnitude);
        }
    }

    public void MoverA(Vector3 destino, bool corriendo = false)
    {
        agente.isStopped = false;
        agente.speed = corriendo ? velocidadAlta : velocidadBaja;
        agente.SetDestination(destino);
    }

    public void Frenar()
    {
        agente.isStopped = true;
        agente.ResetPath(); // Borra el camino para no resbalar
    }

    // Funciones útiles para el Cerebro
    public bool HaLlegado(float margen = 1.0f)
    {
        // Solo comprueba la distancia matemática real 
        return !agente.pathPending && agente.remainingDistance <= margen;
    }

    public bool TieneCaminoCortado()
    {
        return !agente.pathPending && agente.pathStatus == NavMeshPathStatus.PathPartial;
    }
}