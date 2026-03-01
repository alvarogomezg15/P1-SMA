using UnityEngine;

public class CamaraRoblox : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform objetivo; // Arrastra a Link (el padre) aquí
    public SkinnedMeshRenderer cuerpoPersonaje; // ¡IMPORTANTE! Arrastra aquí la piel (ChildLink)

    [Header("Configuración")]
    public float distancia = 5.0f;       // Distancia inicial
    public float altura = 1.4f;          // Altura a la que mira (cabeza)
    public float velocidadZoom = 5.0f;
    public float sensibilidadRaton = 3.0f;
    public float distanciaMinima = 0.5f; // A partir de aquí entra en 1ª persona
    public float distanciaMaxima = 20.0f;

    // Variables internas
    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        // Evita saltos bruscos al empezar
        Vector3 angulos = transform.eulerAngles;
        currentX = angulos.y;
        currentY = angulos.x;
    }

    void LateUpdate()
    {
        if (!objetivo) return;

        // --- 1. ZOOM (Rueda del ratón) ---
        distancia -= Input.GetAxis("Mouse ScrollWheel") * velocidadZoom;
        distancia = Mathf.Clamp(distancia, 0.0f, distanciaMaxima);

        // --- 2. LOGICA DE DESAPARECER (1ª Persona) ---
        // Si la distancia es menor que el mínimo, forzamos a 0 y ocultamos la malla
        if (distancia < distanciaMinima)
        {
            distancia = 0.0f; 
            if (cuerpoPersonaje != null) cuerpoPersonaje.enabled = false; // ¡Desaparece!
        }
        else
        {
            if (cuerpoPersonaje != null) cuerpoPersonaje.enabled = true; // ¡Reaparece!
        }

        // --- 3. ROTACIÓN (Orbitar) ---
        // Si estamos en 1ª persona (distancia 0) O pulsamos clic derecho
        if (distancia == 0.0f || Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * sensibilidadRaton;
            currentY -= Input.GetAxis("Mouse Y") * sensibilidadRaton;
            
            // Limitamos para no dar la vuelta completa verticalmente
            currentY = Mathf.Clamp(currentY, -30, 80);
        }

        // --- 4. APLICAR MOVIMIENTO ---
        Vector3 direccion = new Vector3(0, 0, -distancia);
        Quaternion rotacion = Quaternion.Euler(currentY, currentX, 0);

        // Posición final = Objetivo + Altura + (Rotación * Distancia)
        transform.position = objetivo.position + Vector3.up * altura + rotacion * direccion;
        
        // La cámara siempre mira a la cabeza de Link
        transform.LookAt(objetivo.position + Vector3.up * altura);
    }
}