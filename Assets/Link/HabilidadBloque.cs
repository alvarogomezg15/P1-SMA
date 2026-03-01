using UnityEngine;
using System.Collections;

public class HabilidadBloque : MonoBehaviour
{
    [Header("Configuración del Bloque")]
    public float tiempoActivo = 5f;
    public float radioMaximoActivacion = 10f; 

    [Header("Sistema de Enfriamiento")]
    public float tiempoEnfriamiento = 30f; // Los 30 segundos que pediste
    private float tiempoSiguienteUso = 0f; // Marca de tiempo para el próximo uso permitido

    [Header("Asigna aquí tus bloques de la escena")]
    public GameObject[] misBloques; 

    void Update()
    {
        // Al pulsar la Q, primero comprobamos si el enfriamiento ha pasado
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time >= tiempoSiguienteUso)
            {
                ActivarBloqueCercano();
            }
            else
            {
                // Calculamos cuánto falta para avisar al jugador
                float tiempoRestante = tiempoSiguienteUso - Time.time;
                Debug.Log("⏳ Habilidad en enfriamiento. Faltan: " + tiempoRestante.ToString("F1") + " segundos.");
            }
        }
    }

    void ActivarBloqueCercano()
    {
        if (misBloques == null || misBloques.Length == 0) return;

        GameObject bloqueMasCercano = null;
        float distanciaMinima = float.MaxValue;

        // 1. Buscamos el bloque inactivo más cercano dentro del rango
        foreach (GameObject bloque in misBloques)
        {
            if (bloque != null && !bloque.activeInHierarchy)
            {
                float distancia = Vector3.Distance(transform.position, bloque.transform.position);
                
                if (distancia < distanciaMinima && distancia <= radioMaximoActivacion)
                {
                    distanciaMinima = distancia;
                    bloqueMasCercano = bloque;
                }
            }
        }

        // 2. Si encontramos uno, lo activamos y EMPIEZA el enfriamiento
        if (bloqueMasCercano != null)
        {
            bloqueMasCercano.SetActive(true);
            Debug.Log("🧊 Bloque ACTIVADO. ¡Enfriamiento de 30s iniciado!");

            // Marcamos cuándo podrá volver a usarse la habilidad
            tiempoSiguienteUso = Time.time + tiempoEnfriamiento;

            StartCoroutine(DesactivarBloque(bloqueMasCercano, tiempoActivo));
        }
        else
        {
            Debug.Log("❌ No hay bloques cerca para activar. (No se gasta el enfriamiento)");
        }
    }

    IEnumerator DesactivarBloque(GameObject bloque, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        bloque.SetActive(false);
        Debug.Log("🧊 Bloque desactivado automáticamente.");
    }
}
