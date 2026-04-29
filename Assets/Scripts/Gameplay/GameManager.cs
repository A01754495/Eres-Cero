using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Sesión del jugador ---
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

    // --- Retroalimentación al perder ---
    public int    UltimoValorBase  { get; set; } = 0;
    public string UltimoOperador   { get; set; } = "";
    public int    UltimoNumero     { get; set; } = 0;
    public int    UltimoResultado  { get; set; } = 0;
    public int    MetaFallida      { get; set; } = 0;

    // --- Tutorial ---
    public bool EsTutorial       { get; set; } = false;
    public bool EsPrimeraPartida { get; set; } = false;

    public int ValorInicioOla { get; set; } = 0;

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
        SkinSeleccionada = PlayerPrefs.GetInt("SkinSeleccionada", 0);
        CargarSesionGuardada();
    }

    // ── Sesión persistente ──────────────────────────────────────────
    public void GuardarSesion()
    {
        PlayerPrefs.SetInt("SesionIdJugador", IdJugador);
        PlayerPrefs.SetString("SesionAlias", AliasJugador);
        PlayerPrefs.Save();
    }

    void CargarSesionGuardada()
    {
        int idGuardado = PlayerPrefs.GetInt("SesionIdJugador", -1);
        if (idGuardado > 0)
        {
            IdJugador    = idGuardado;
            AliasJugador = PlayerPrefs.GetString("SesionAlias", "");
        }
    }

    void BorrarSesionGuardada()
    {
        PlayerPrefs.DeleteKey("SesionIdJugador");
        PlayerPrefs.DeleteKey("SesionAlias");
        PlayerPrefs.Save();
    }

    // ── Estadísticas para logros ────────────────────────────────────
    // Llámalo en GameOverController junto a AcumularPuntaje()
    public void GuardarEstadisticasLogros()
    {
        // Logro1: puertas en difícil en UNA sola partida
        if (Dificultad == "dificil")
        {
            int mejorPuertasDificil = PlayerPrefs.GetInt("MejorPuertasDificil", 0);
            if (PuertasVivas > mejorPuertasDificil)
                PlayerPrefs.SetInt("MejorPuertasDificil", PuertasVivas);
        }

        // Logro3: mejor puntaje en una sola sesión
        int mejorPuntaje = PlayerPrefs.GetInt("MejorPuntaje", 0);
        if (Puntaje > mejorPuntaje)
            PlayerPrefs.SetInt("MejorPuntaje", Puntaje);

        // Logro4: mejor tiempo en una sola partida (en segundos)
        float mejorTiempo = PlayerPrefs.GetFloat("MejorTiempo", 0f);
        if (TiempoPartida > mejorTiempo)
            PlayerPrefs.SetFloat("MejorTiempo", TiempoPartida);

        // Logro6: puntaje total acumulado (ya lo hace AcumularPuntaje, solo lo leemos)

        PlayerPrefs.Save();
    }

    // Llámalo en AspectosController cuando el jugador desbloquea una skin
    public void RegistrarSkinsDesbloqueadas(int[] puntajesDesbloqueo)
    {
        int puntajeTotal = PlayerPrefs.GetInt("PuntajeTotal", 0);
        int count = 0;
        foreach (int req in puntajesDesbloqueo)
            if (puntajeTotal >= req) count++;
        PlayerPrefs.SetInt("SkinsDesbloqueadas", count);
        PlayerPrefs.Save();
    }

    public void IniciarPartida(string dificultad)
    {
        Dificultad    = dificultad;
        ValorJugador  = 0;
        ValorBase     = 0;
        Puntaje       = 0;
        PuertasVivas  = 0;
        TiempoPartida = 0f;

        UltimoValorBase = 0;
        UltimoOperador  = "";
        UltimoNumero    = 0;
        UltimoResultado = 0;
        MetaFallida     = 0;

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
        BorrarSesionGuardada();
    }
}