using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string Dificultad    { get; set; } = "facil";
    public int    ValorJugador  { get; set; } = 0;
    public int    Puntaje       { get; set; } = 0;
    public int    PuertasVivas  { get; set; } = 0;
    public int    ValorBase     { get; set; } = 0;

    public float VelocidadBase  { get; private set; } = 3f;
    public float IncrementoVel  { get; private set; } = 0.15f;
    public int   RangoOperacion { get; private set; } = 5;

    // Tutorial
    public bool EsTutorial { get; set; } = false;

    // Primera partida — guardado localmente con PlayerPrefs
    public bool EsPrimeraPartida => PlayerPrefs.GetInt("HaJugado", 0) == 0;

    public void MarcarPartidaJugada()
    {
        PlayerPrefs.SetInt("HaJugado", 1);
        PlayerPrefs.Save();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IniciarPartida(string dificultad)
    {
        Dificultad   = dificultad;
        ValorJugador = 0;
        ValorBase    = 0;
        Puntaje      = 0;
        PuertasVivas = 0;

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

    public float VelocidadActual()
    {
        return VelocidadBase + IncrementoVel * PuertasVivas;
    }
}