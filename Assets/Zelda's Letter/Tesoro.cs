using UnityEngine;

public class Tesoro : MonoBehaviour
{
    [HideInInspector] public Vector3 posicionOriginal;
    private bool recolectado = false;

    void Start()
    {
        // Memoriza dónde empezó la partida para poder volver si atrapan a Link
        posicionOriginal = transform.position;
    }

    void Update()
    {
        // Efecto visual clásico: Rotar sobre sí mismo
        transform.Rotate(Vector3.up * 90f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !recolectado)
        {
            recolectado = true;
            
            // Le decimos al inventario de Link que sume 1
            other.GetComponent<SistemaTesoros>().RecogerTesoro(this);
            
            // Apagamos el tesoro (desaparece visual y físicamente)
            gameObject.SetActive(false);
        }
    }

    // Función para cuando el guardia te pilla
    public void Resetear()
    {
        recolectado = false;
        transform.position = posicionOriginal;
        gameObject.SetActive(true); // ¡Vuelve a aparecer!
    }
}