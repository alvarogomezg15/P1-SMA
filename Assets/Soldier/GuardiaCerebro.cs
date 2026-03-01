using UnityEngine;

public class GuardiaCerebro : MonoBehaviour
{
    [HideInInspector] public GuardiaMotor motor;
    [HideInInspector] public GuardiaSensores sensores;

    [HideInInspector] public ComportamientoPatrulla modPatrulla;
    [HideInInspector] public ComportamientoPersecucion modPersecucion;
    [HideInInspector] public ComportamientoBusqueda modBusqueda;
    [HideInInspector] public ComportamientoSorpresa modSorpresa;
    [HideInInspector] public ComportamientoAlarma modAlarma;

    [Header("Castigo")]
    public Transform destinoLobby;

    private ComportamientoGuardia comportamientoActual;

    void Awake()
    {
        motor = GetComponent<GuardiaMotor>();
        sensores = GetComponent<GuardiaSensores>();

        modPatrulla = GetComponent<ComportamientoPatrulla>();
        modPersecucion = GetComponent<ComportamientoPersecucion>();
        modBusqueda = GetComponent<ComportamientoBusqueda>();
        modSorpresa = GetComponent<ComportamientoSorpresa>();
        modAlarma = GetComponent<ComportamientoAlarma>();

        if (modPatrulla != null) modPatrulla.cerebro = this;
        if (modPersecucion != null) modPersecucion.cerebro = this;
        if (modBusqueda != null) modBusqueda.cerebro = this;
        if (modSorpresa != null) modSorpresa.cerebro = this;
        if (modAlarma != null) modAlarma.cerebro = this;
    }

    void Start()
    {
        CambiarComportamiento(modPatrulla);
    }

    void Update()
    {
        if (comportamientoActual != null)
        {
            comportamientoActual.Ejecutar();
        }
    }

    public void CambiarComportamiento(ComportamientoGuardia nuevoComportamiento)
    {
        if (comportamientoActual != null) comportamientoActual.Salir();
        comportamientoActual = nuevoComportamiento;
        if (comportamientoActual != null) comportamientoActual.Entrar();
    }
}