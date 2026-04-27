// Clase de lógica pura — no hereda de MonoBehaviour
// Coloca este archivo en Assets/Scripts/Logic/
public class LogicaJuego
{
    // Estado de la partida
    public string Dificultad    { get; private set; } = "facil";
    public int    Puntaje       { get; set; } = 0;
    public int    ValorJugador  { get; set; } = 0;
    public int    ValorBase     { get; set; } = 0;
    public int    PuertasVivas  { get; set; } = 0;
    public float  TiempoPartida { get; set; } = 0f;
    public int    IdJugador     { get; set; } = -1;
    public string AliasJugador  { get; set; } = "";

    // Parámetros de dificultad
    public float VelocidadBase  { get; private set; } = 1.5f;
    public float IncrementoVel  { get; private set; } = 0.10f;
    public int   RangoOperacion { get; private set; } = 50;

    // Puntaje histórico acumulado (simula PlayerPrefs en pruebas)
    public int PuntajeTotal { get; private set; } = 0;

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
                RangoOperacion = 50;
                break;
            case "medio":
                VelocidadBase  = 2.5f;
                IncrementoVel  = 0.15f;
                RangoOperacion = 100;
                break;
            case "dificil":
                VelocidadBase  = 3.5f;
                IncrementoVel  = 0.25f;
                RangoOperacion = 100;
                break;
        }
    }

    public float VelocidadActual() => VelocidadBase + IncrementoVel * PuertasVivas;

    public void AcumularPuntaje(int puntajeActual)
    {
        PuntajeTotal += puntajeActual;
    }

    public void CerrarSesion()
    {
        IdJugador    = -1;
        AliasJugador = "";
    }

    public int PuntajePorPuerta(string dificultad, int puertasVivas)
    {
        int multiplicador = dificultad switch
        {
            "medio"   => 20,
            "dificil" => 30,
            _         => 10
        };
        return 100 + puertasVivas * multiplicador;
    }

    public bool VerificarPuerta(int valorJugador, int numeroMeta)
        => valorJugador == numeroMeta;

    public int AplicarOperacion(int valorActual, string operador, int numero)
    {
        return operador switch
        {
            "+" => valorActual + numero,
            "-" => valorActual - numero,
            "*" => valorActual * numero,
            "/" => numero != 0 ? valorActual / numero : valorActual,
            _   => valorActual
        };
    }
}
