using UnityEngine;

// GameManager persiste entre escenas y guarda el estado global de la partida.
// GameManager.Instance desde cualquier script.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Datos que vienen de la pantalla de Dificultad ---
    public string Dificultad { get; set; } = "facil"; // "facil" | "medio" | "dificil"

    // --- Estado de la partida activa ---
    public int    ValorJugador  { get; set; } = 0;
    public int    Puntaje       { get; set; } = 0;
    public int    PuertasVivas  { get; set; } = 0; // cuántas puertas cruzó correctamente

    // --- Parámetros que cambian según dificultad ---
    // Velocidad inicial del scroll del escenario
    public float VelocidadBase  { get; private set; } = 3f;
    // Cuánto aumenta la velocidad por puerta superada
    public float IncrementoVel  { get; private set; } = 0.15f;
    // Rango de los números que aparecen en las casillas de operación
    public int   RangoOperacion { get; private set; } = 5;

    void Awake()
    {
        // Patrón Singleton: solo existe una instancia y sobrevive a cambios de escena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Llama esto desde DificultadController antes de cargar Gameplay
    public void IniciarPartida(string dificultad)
    {
        Dificultad    = dificultad;
        ValorJugador  = 0;
        Puntaje       = 0;
        PuertasVivas  = 0;

        switch (dificultad)
        {
            case "facil":
                VelocidadBase  = 2.5f;
                IncrementoVel  = 0.10f;
                RangoOperacion = 5;
                break;
            case "medio":
                VelocidadBase  = 3.5f;
                IncrementoVel  = 0.15f;
                RangoOperacion = 10;
                break;
            case "dificil":
                VelocidadBase  = 5f;
                IncrementoVel  = 0.25f;
                RangoOperacion = 20;
                break;
        }
    }

    // Velocidad actual = base + incremento * puertas cruzadas
    public float VelocidadActual()
    {
        return VelocidadBase + IncrementoVel * PuertasVivas;
    }
}
