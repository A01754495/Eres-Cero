using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Sesión del jugador ---
    // TODO BACKEND: Llenar al hacer login/registro exitoso
    public int    IdJugador      { get; set; } = -1;
    public string AliasJugador   { get; set; } = "";
    public bool   HaySesion      => IdJugador > 0;

    // --- Skin seleccionada (0 = default) ---
    public int SkinSeleccionada  { get; set; } = 0;

    // --- Dificultad ---
    public string Dificultad     { get; set; } = "facil";

    // --- Estado de la partida activa ---
    public int   ValorJugador    { get; set; } = 0;
    public int   Puntaje         { get; set; } = 0;
    public int   PuertasVivas    { get; set; } = 0;
    public int   ValorBase       { get; set; } = 0;
    public float TiempoPartida   { get; set; } = 0f;

    // --- Parámetros de dificultad ---
    public float VelocidadBase   { get; private set; } = 3f;
    public float IncrementoVel   { get; private set; } = 0.15f;
    public int   RangoOperacion  { get; private set; } = 5;

    // --- Tutorial ---
    public bool EsTutorial       { get; set; } = false;
    public bool EsPrimeraPartida => PlayerPrefs.GetInt("HaJugado", 0) == 0;

    public void MarcarPartidaJugada()
    {
        PlayerPrefs.SetInt("HaJugado", 1);
        PlayerPrefs.Save();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar skin guardada
        SkinSeleccionada = PlayerPrefs.GetInt("SkinSeleccionada", 0);
    }

    public void IniciarPartida(string dificultad)
    {
        Dificultad    = dificultad;
        ValorJugador  = 0;
        ValorBase     = 0;
        Puntaje       = 0;
        PuertasVivas  = 0;
        TiempoPartida = 0f;

        switch (dificultad)
        {
            case "facil":
                VelocidadBase  = 1.5f;
                IncrementoVel  = 0.10f;
                RangoOperacion = 5;
                break;
            case "medio":
                VelocidadBase  = 2.5f;
                IncrementoVel  = 0.15f;
                RangoOperacion = 10;
                break;
            case "dificil":
                VelocidadBase  = 3.5f;
                IncrementoVel  = 0.25f;
                RangoOperacion = 20;
                break;
        }
    }

    public float VelocidadActual() => VelocidadBase + IncrementoVel * PuertasVivas;

    public void ActualizarTiempo() => TiempoPartida += Time.deltaTime;

    // Acumula el puntaje de la partida al histórico local (para desbloqueo de aspectos)
    public void AcumularPuntaje()
    {
        int total = PlayerPrefs.GetInt("PuntajeTotal", 0) + Puntaje;
        PlayerPrefs.SetInt("PuntajeTotal", total);
        PlayerPrefs.Save();
    }

    public void CerrarSesion()
    {
        IdJugador    = -1;
        AliasJugador = "";
    }
}