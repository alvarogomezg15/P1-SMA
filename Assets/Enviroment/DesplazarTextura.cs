using UnityEngine;

public class DesplazarTextura : MonoBehaviour
{
    // Estas variables aparecerán en el Inspector para que controles la velocidad
    public float velocidadX = 0.5f;
    public float velocidadY = 0.0f;

    private Renderer miRenderer;

    void Start()
    {
        // Esto busca el componente que dibuja la textura al iniciar el juego
        miRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Calculamos cuánto se ha movido basado en el tiempo que lleva el juego abierto
        float moverX = Time.time * velocidadX;
        float moverY = Time.time * velocidadY;

        // Le aplicamos ese movimiento al material
        miRenderer.material.mainTextureOffset = new Vector2(moverX, moverY);
    }
}