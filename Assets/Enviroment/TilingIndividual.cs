using UnityEngine;

[ExecuteInEditMode] // ¡Funciona en el editor sin dar al Play!
public class TilingIndividual : MonoBehaviour
{
    // Esto aparecerá en el inspector del objeto
    [Header("Ajustes de Textura")]
    public float TilingX = 1.0f;
    public float TilingY = 1.0f;
    public float OffsetX = 0.0f;
    public float OffsetY = 0.0f;

    Renderer _renderer;
    MaterialPropertyBlock _propBlock;

    void OnValidate() // Se ejecuta cada vez que tocas algo en el inspector
    {
        UpdateTiling();
    }

    void Start()
    {
        UpdateTiling();
    }

    void UpdateTiling()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        // Obtenemos los valores actuales (para no borrar otras cosas como el color)
        _renderer.GetPropertyBlock(_propBlock);

        // Creamos el vector con tus ajustes (Tiling X, Tiling Y, Offset X, Offset Y)
        Vector4 st = new Vector4(TilingX, TilingY, OffsetX, OffsetY);

        // Asignamos al nombre estándar de Unity (_BaseMap_ST para URP, _MainTex_ST para Standard)
        // Probamos ambos por si acaso cambias de shader
        _propBlock.SetVector("_BaseMap_ST", st); 
        _propBlock.SetVector("_MainTex_ST", st);

        // Aplicamos los cambios SOLO a este objeto
        _renderer.SetPropertyBlock(_propBlock);
    }
}