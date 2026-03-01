using UnityEngine;

public class TrampaLobby : MonoBehaviour
{
    [Header("Objetos a hacer aparecer")]
    public GameObject[] vallas;
    
    [HideInInspector] public bool estaActivada = false;

    public void ActivarVallas()
    {
        if (estaActivada) return;
        
        estaActivada = true;
        foreach (GameObject valla in vallas)
        {
            valla.SetActive(true); // Hace aparecer las vallas 
        }
        Debug.Log("Vallas de seguridad activadas");
    }

    public void DesactivarVallas()
    {
        estaActivada = false;
        foreach (GameObject valla in vallas)
        {
            valla.SetActive(false); // Las esconde al resetear
        }
    }
}