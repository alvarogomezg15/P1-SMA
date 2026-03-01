using UnityEngine;

public class CerebroLink : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadAndar = 3.5f;
    public float velocidadCorrer = 4.5f;
    public float velocidadGiro = 15.0f; // Qué tan rápido se da la vuelta

    [Header("Referencias")]
    public Transform camara; 

    Animator animator;
    Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Congelamos la rotación física para que Link no vuelque al chocar
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Si se te olvida poner la cámara manualmentes, la buscamos sola
        if (camara == null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. INPUTS (Teclas)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool corriendo = Input.GetKey(KeyCode.LeftShift);

        // 2. CALCULAR DIRECCIÓN SEGÚN LA CÁMARA
        // Obtenemos hacia dónde mira la cámara
        Vector3 camForward = camara.forward;
        Vector3 camRight = camara.right;

        // "Aplastamos" la altura (Y) a 0 para que no intente volar ni meterse bajo tierra
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Creamos la dirección final sumando hacia donde mira la cámara y hacia donde mira su derecha
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 3. MOVER Y GIRAR
        if (moveDir.magnitude >= 0.1f)
        {
            // A. Girar suavemente hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, velocidadGiro * Time.deltaTime);

            // B. Decidir velocidad
            float currentSpeed = corriendo ? velocidadCorrer : velocidadAndar;

            // C. Mover hacia adelante (Z local)
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            // D. Avisar al Animator para que cambie la animación
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            // Si no tocamos teclas, velocidad 0 (Idle)
            animator.SetFloat("Speed", 0);
        }
    }
}