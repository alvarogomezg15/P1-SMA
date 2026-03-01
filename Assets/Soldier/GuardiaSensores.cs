using UnityEngine;
using UnityEngine.AI; 

public class GuardiaSensores : MonoBehaviour
{
    [Header("Visión")]
    public Transform objetivo; 
    public float radioVision = 12f;
    public float anguloVision = 45f;

    [Header("Oído")]
    public float radioOido = 8f; 
    public float umbralRuido = 4.0f; 

    [Header("Protección de Tesoro")]
    public Tesoro tesoroAsignado; 

    [HideInInspector] public Vector3 ultimaPosicionConocida;
    
    private Vector3 posAnteriorLink;
    private float velocidadLink;

    void Update()
    {
        if (objetivo != null)
        {
            velocidadLink = (objetivo.position - posAnteriorLink).magnitude / Time.deltaTime;
            posAnteriorLink = objetivo.position;
        }
    }

    public bool VeAlJugador()
    {
        if (objetivo == null) return false;

        Vector3 direccionHaciaLink = objetivo.position - transform.position;
        float distancia = direccionHaciaLink.magnitude;

        if (distancia < radioVision && Vector3.Angle(transform.forward, direccionHaciaLink) < anguloVision)
        {
            Vector3 origen = transform.position + Vector3.up * 1f;
            Vector3 destino = objetivo.position + Vector3.up * 1f;
            
            RaycastHit impacto;
            if (Physics.Raycast(origen, destino - origen, out impacto, radioVision))
            {
                if (impacto.collider.CompareTag("Player"))
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(objetivo.position, out hit, 2.0f, NavMesh.AllAreas))
                        ultimaPosicionConocida = hit.position;
                    else
                        ultimaPosicionConocida = objetivo.position;
                        
                    return true; 
                }
            }
        }
        return false; 
    }

    public bool EscuchaAlJugador(out Vector3 puntoEstimado)
    {
        puntoEstimado = Vector3.zero;
        if (objetivo == null) return false;

        if (velocidadLink >= umbralRuido)
        {
            if (Vector3.Distance(transform.position, objetivo.position) <= radioOido)
            {
                Vector3 direccionSonido = (objetivo.position - transform.position).normalized;
                Vector3 puntoTeorico = transform.position + (direccionSonido * 6f);
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(puntoTeorico, out hit, 4.0f, NavMesh.AllAreas))
                {
                    puntoEstimado = hit.position;
                }
                else
                {
                    puntoEstimado = puntoTeorico; 
                }
                
                return true;
            }
        }
        return false;
    }

    public float DistanciaAlJugador()
    {
        if (objetivo == null) return 999f;
        return Vector3.Distance(transform.position, objetivo.position);
    }

    // Vigilancia tesoro (el sensor)
    public bool DescubreRobo()
    {
        if (tesoroAsignado == null) return false;

        if (!tesoroAsignado.gameObject.activeInHierarchy)
        {
            // Solo salta la alarma si pasa a menos de 8 metros
            if (Vector3.Distance(transform.position, tesoroAsignado.posicionOriginal) < 8f)
            {
                return true;
            }
        }
        return false;
    }
}