using NUnit.Framework;

public class EresCeroTests
{
    // ================================================================
    // UT-01 — GameManager: IniciarPartida("facil")
    // Iniciar partida fácil reinicia todos los valores correctamente
    // ================================================================
    [Test]
    public void UT01_IniciarPartida_Facil_ReiniciaValoresCorrectos()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.Puntaje = 999; gm.ValorJugador = 5; gm.PuertasVivas = 10;

        // Act
        gm.IniciarPartida("facil");

        // Assert
        Assert.AreEqual(0,    gm.Puntaje);
        Assert.AreEqual(0,    gm.ValorJugador);
        Assert.AreEqual(0,    gm.PuertasVivas);
        Assert.AreEqual(0f,   gm.TiempoPartida);
        Assert.AreEqual(1.5f, gm.VelocidadBase);
        Assert.AreEqual(50,   gm.RangoOperacion);
    }

    // ================================================================
    // UT-02 — GameManager: IniciarPartida("dificil")
    // Iniciar partida difícil establece velocidad 3.5 y rango 100
    // ================================================================
    [Test]
    public void UT02_IniciarPartida_Dificil_EstableceVelocidadYRangoCorrectos()
    {
        // Arrange
        var gm = new LogicaJuego();

        // Act
        gm.IniciarPartida("dificil");

        // Assert
        Assert.AreEqual(3.5f, gm.VelocidadBase);
        Assert.AreEqual(100,  gm.RangoOperacion);
    }

    // ================================================================
    // UT-03 — GameManager: VelocidadActual()
    // Con 5 puertas cruzadas en fácil la velocidad es 2.0 (1.5 + 0.1×5)
    // ================================================================
    [Test]
    public void UT03_VelocidadActual_Facil_Con5Puertas_Retorna2()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.IniciarPartida("facil");
        gm.PuertasVivas = 5;

        // Act
        float vel = gm.VelocidadActual();

        // Assert
        Assert.AreEqual(2.0f, vel);
    }

    // ================================================================
    // UT-04 — GameManager: VelocidadActual()
    // Con 0 puertas devuelve exactamente la velocidad base
    // ================================================================
    [Test]
    public void UT04_VelocidadActual_Medio_Con0Puertas_RetornaVelocidadBase()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.IniciarPartida("medio");
        gm.PuertasVivas = 0;

        // Act
        float vel = gm.VelocidadActual();

        // Assert
        Assert.AreEqual(2.5f, vel);
    }

    // ================================================================
    // UT-05 — GameManager: AcumularPuntaje()
    // Suma correctamente el puntaje de la partida al histórico existente
    // ================================================================
    [Test]
    public void UT05_AcumularPuntaje_SumaCorrectamenteAlHistorico()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.AcumularPuntaje(200); // simular histórico previo
        gm.Puntaje = 150;

        // Act
        gm.AcumularPuntaje(gm.Puntaje);

        // Assert
        Assert.AreEqual(350, gm.PuntajeTotal);
    }

    // ================================================================
    // UT-06 — GameManager: AcumularPuntaje()
    // Con puntaje 0 no modifica el total histórico
    // ================================================================
    [Test]
    public void UT06_AcumularPuntaje_Con0_NoModificaHistorico()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.AcumularPuntaje(500);
        gm.Puntaje = 0;

        // Act
        gm.AcumularPuntaje(gm.Puntaje);

        // Assert
        Assert.AreEqual(500, gm.PuntajeTotal);
    }

    // ================================================================
    // UT-07 — GameManager: CerrarSesion()
    // IdJugador es -1 y AliasJugador es vacío después de cerrar sesión
    // ================================================================
    [Test]
    public void UT07_CerrarSesion_LimpiaIdJugadorYAlias()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.IdJugador    = 5;
        gm.AliasJugador = "Ian";

        // Act
        gm.CerrarSesion();

        // Assert
        Assert.AreEqual(-1, gm.IdJugador);
        Assert.AreEqual("", gm.AliasJugador);
    }

    // ================================================================
    // UT-08 — LoginController: ValidarAlias()
    // Alias válido con letras y números retorna verdadero sin error
    // ================================================================
    [Test]
    public void UT08_ValidarAlias_AliasValido_RetornaTrue()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAlias("Jugador123", out string error, esLogin: false);

        // Assert
        Assert.IsTrue(resultado);
        Assert.IsNull(error);
    }

    // ================================================================
    // UT-09 — LoginController: ValidarAlias()
    // Alias vacío retorna falso con mensaje de error
    // ================================================================
    [Test]
    public void UT09_ValidarAlias_AliasVacio_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAlias("", out string error, esLogin: false);

        // Assert
        Assert.IsFalse(resultado);
        Assert.IsNotNull(error);
    }

    // ================================================================
    // UT-10 — LoginController: ValidarAlias()
    // Alias con más de 20 caracteres retorna falso
    // ================================================================
    [Test]
    public void UT10_ValidarAlias_AliasMasDe20Chars_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAlias("AliasDemasiadoLargoXYZ", out string error, esLogin: false);

        // Assert
        Assert.IsFalse(resultado);
    }

    // ================================================================
    // UT-11 — LoginController: ValidarAlias()
    // Alias con palabra altisonante retorna falso
    // ================================================================
    [Test]
    public void UT11_ValidarAlias_AliasAltisonante_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAlias("superverga99", out string error, esLogin: false);

        // Assert
        Assert.IsFalse(resultado);
    }

    // ================================================================
    // UT-12 — LoginController: ValidarNipLogin()
    // NIP de exactamente 4 dígitos numéricos retorna verdadero
    // ================================================================
    [Test]
    public void UT12_ValidarNip_Nip4Digitos_RetornaTrue()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarNipLogin("4827", out string error);

        // Assert
        Assert.IsTrue(resultado);
    }

    // ================================================================
    // UT-13 — LoginController: ValidarNipLogin()
    // NIP con letras retorna falso
    // ================================================================
    [Test]
    public void UT13_ValidarNip_NipConLetras_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarNipLogin("12ab", out string error);

        // Assert
        Assert.IsFalse(resultado);
    }

    // ================================================================
    // UT-14 — LoginController: ValidarNipLogin()
    // NIP de solo 3 dígitos retorna falso
    // ================================================================
    [Test]
    public void UT14_ValidarNip_Nip3Digitos_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarNipLogin("123", out string error);

        // Assert
        Assert.IsFalse(resultado);
    }

    // ================================================================
    // UT-15 — LoginController: ValidarAnio()
    // Año válido para un niño de 8 años retorna verdadero
    // ================================================================
    [Test]
    public void UT15_ValidarAnio_AnioValido_RetornaTrue()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAnio("2018", out string error);

        // Assert
        Assert.IsTrue(resultado);
    }

    // ================================================================
    // UT-16 — LoginController: ValidarAnio()
    // Año futuro retorna falso con mensaje de error
    // ================================================================
    [Test]
    public void UT16_ValidarAnio_AnioFuturo_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAnio("2030", out string error);

        // Assert
        Assert.IsFalse(resultado);
        Assert.IsNotNull(error);
    }

    // ================================================================
    // UT-17 — LoginController: ValidarAnio()
    // Texto no numérico en el año retorna falso
    // ================================================================
    [Test]
    public void UT17_ValidarAnio_TextoNoNumerico_RetornaFalse()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        bool resultado = v.ValidarAnio("dos mil", out string error);

        // Assert
        Assert.IsFalse(resultado);
    }

    // ================================================================
    // UT-18 — PlayerController: Puntaje por puerta
    // Primera puerta otorga exactamente 100 puntos
    // ================================================================
    [Test]
    public void UT18_PuntajePorPuerta_PrimeraPuerta_Retorna100()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.IniciarPartida("facil");

        // Act
        int pts = gm.PuntajePorPuerta("facil", gm.PuertasVivas);

        // Assert
        Assert.AreEqual(100, pts);
    }

    // ================================================================
    // UT-19 — PlayerController: Puntaje por puerta
    // Décima puerta otorga 190 puntos (100 + 9×10)
    // ================================================================
    [Test]
    public void UT19_PuntajePorPuerta_DecimaPuerta_Retorna190()
    {
        // Arrange
        var gm = new LogicaJuego();
        gm.IniciarPartida("facil");
        gm.PuertasVivas = 9;

        // Act
        int pts = gm.PuntajePorPuerta("facil", gm.PuertasVivas);

        // Assert
        Assert.AreEqual(190, pts);
    }

    // ================================================================
    // UT-20 — LoginController: GenerarNuevoNip()
    // El NIP generado siempre tiene 4 dígitos (entre 1000 y 9999)
    // ================================================================
    [Test]
    public void UT20_GenerarNip_SiempreTiene4Digitos()
    {
        // Arrange
        var v = new ValidadorLogin();

        // Act
        int nip = v.GenerarNip();

        // Assert
        Assert.IsTrue(nip >= 1000 && nip <= 9999);
    }
}
