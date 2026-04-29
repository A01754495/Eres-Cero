using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Carriles (posición X de cada uno)")]
    public float carrilIzquierda = -2.5f;
    public float carrilCentro   =  0f;
    public float carrilDerecha  =  2.5f;

    [Header("Velocidad de transición entre carriles")]
    public float velocidadMovimiento = 12f;

    [Header("Dash — adelantar con espacio")]
    public float multiplicadorDash = 4f;
    public float duracionDash      = 1.2f;

    [Header("UI: texto que muestra el valor del jugador")]
    public TextMeshPro textoValor;
    public NumeroDisplay numeroDisplay;

    // AUDIO
    private PlayerAudio playerAudio;

    private int   carrilActual = 1;
    private float xObjetivo;
    private bool  puedeMover = true;

    private InputAction moverIzquierda;
    private InputAction moverDerecha;
    private InputAction accionDash;

    public bool puedeCambiarCarril = true;

    private bool  dashActivo = false;
    private float timerDash  = 0f;

    // Para evitar doble trigger en la puerta
    private bool perdiendo = false;

    private Color[] colores = new Color[]
    {
        Color.red, Color.blue, Color.green, Color.yellow,
        Color.cyan, Color.magenta,
        new Color(1f, 0.5f, 0f),
        new Color(0.6f, 0.2f, 1f),
        new Color(1f, 0.8f, 0.2f),
        Color.white, Color.gray, Color.black
    };

    void Awake()
    {
        moverIzquierda = new InputAction(type: InputActionType.Button);
        moverIzquierda.AddBinding("<Keyboard>/leftArrow");
        moverIzquierda.AddBinding("<Keyboard>/a");

        moverDerecha = new InputAction(type: InputActionType.Button);
        moverDerecha.AddBinding("<Keyboard>/rightArrow");
        moverDerecha.AddBinding("<Keyboard>/d");

        accionDash = new InputAction(type: InputActionType.Button);
        accionDash.AddBinding("<Keyboard>/space");
    }

    void OnEnable()
    {
        moverIzquierda.Enable();
        moverDerecha.Enable();
        accionDash.Enable();

        moverIzquierda.performed += OnMoverIzquierda;
        moverDerecha.performed   += OnMoverDerecha;
        accionDash.performed     += OnDash;
    }

    void OnDisable()
    {
        moverIzquierda.performed -= OnMoverIzquierda;
        moverDerecha.performed   -= OnMoverDerecha;
        accionDash.performed     -= OnDash;

        moverIzquierda.Disable();
        moverDerecha.Disable();
        accionDash.Disable();
    }

    void OnMoverIzquierda(InputAction.CallbackContext ctx) => CambiarCarril(-1);
    void OnMoverDerecha(InputAction.CallbackContext ctx)   => CambiarCarril(1);

    void OnDash(InputAction.CallbackContext ctx)
    {
        if (!puedeMover || dashActivo) return;
        StartCoroutine(EjecutarDash());
    }

    IEnumerator EjecutarDash()
    {
        dashActivo = true;
        timerDash  = duracionDash;

        LaneSpawner spawner = FindFirstObjectByType<LaneSpawner>();
        if (spawner != null)
            spawner.multiplicadorVelocidad = multiplicadorDash;

        while (timerDash > 0f)
        {
            timerDash -= Time.deltaTime;
            yield return null;
        }

        if (spawner != null)
            spawner.multiplicadorVelocidad = 1f;

        dashActivo = false;
    }

    void Start()
    {
        carrilActual = 1;
        xObjetivo    = carrilCentro;

        playerAudio = GetComponent<PlayerAudio>();

        numeroDisplay.MostrarNumero(GameManager.Instance.ValorJugador);
        AplicarSkinGuardada();
    }

    void Update()
    {
        if (!puedeMover) return;
        MoverHaciaCarril();

        if (Mathf.Abs(transform.position.x - xObjetivo) > 0.01f)
            playerAudio.ReproducirMovimiento(true);
        else
            playerAudio.ReproducirMovimiento(false);
    }

    void CambiarCarril(int direccion)
    {
        if (!puedeMover || !puedeCambiarCarril) return;

        carrilActual = Mathf.Clamp(carrilActual + direccion, 0, 2);

        xObjetivo = carrilActual == 0 ? carrilIzquierda
                  : carrilActual == 2 ? carrilDerecha
                  : carrilCentro;
    }

    void MoverHaciaCarril()
    {
        float x = Mathf.MoveTowards(transform.position.x, xObjetivo, velocidadMovimiento * Time.deltaTime);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ── CASILLA DE OPERACIÓN ──
        if (other.CompareTag("Operacion"))
        {
            if (!puedeMover || perdiendo) return;

            CasillaOperacion casilla = other.GetComponent<CasillaOperacion>();
            if (casilla == null) return;

            if (!GameManager.Instance.TocoPrimeraCasilla)
            {
                GameManager.Instance.UltimoValorBase     = GameManager.Instance.ValorJugador;
                GameManager.Instance.TocoPrimeraCasilla  = true;
            }

            GameManager.Instance.UltimoOperador  = casilla.operador;
            GameManager.Instance.UltimoNumero    = casilla.numero;
            GameManager.Instance.UltimoResultado = casilla.resultadoFinal;

            GameManager.Instance.ValorJugador = casilla.resultadoFinal;
            numeroDisplay.MostrarNumero(GameManager.Instance.ValorJugador);

            puedeCambiarCarril = false;
            Destroy(other.gameObject);
        }

        // ── PUERTA ──
        if (other.CompareTag("Puerta"))
        {
            if (perdiendo) return; // evitar doble trigger

            PuertaController puerta = other.GetComponent<PuertaController>();
            if (puerta == null) return;

            if (GameManager.Instance.ValorJugador == puerta.numeroMeta)
            {
                int multiplicador;
                switch (GameManager.Instance.Dificultad)
                {
                    case "medio":   multiplicador = 20; break;
                    case "dificil": multiplicador = 30; break;
                    default:        multiplicador = 10; break;
                }

                GameManager.Instance.Puntaje += 100 + GameManager.Instance.PuertasVivas * multiplicador;

                playerAudio.ReproducirPuntos();

                GameManager.Instance.PuertasVivas += 1;
                GameManager.Instance.ValorJugador  = puerta.numeroMeta;
                GameManager.Instance.ValorBase     = puerta.numeroMeta;

                puerta.MostrarExito();
                numeroDisplay.MostrarNumero(GameManager.Instance.ValorJugador);

                puedeCambiarCarril = true;

                if (dashActivo)
                {
                    StopAllCoroutines();
                    LaneSpawner spawner = FindFirstObjectByType<LaneSpawner>();
                    if (spawner != null) spawner.multiplicadorVelocidad = 1f;
                    dashActivo = false;
                }

                FindFirstObjectByType<LaneSpawner>()?.ForzarNuevaOla();
            }
            else
            {
                perdiendo  = true;
                puedeMover = false;
                puerta.MostrarError();

                // Guardar meta fallida para retroalimentación
                GameManager.Instance.MetaFallida = puerta.numeroMeta;

                playerAudio.ReproducirPerder();
                StartCoroutine(IrRetroalimentacion());
            }
        }
    }

    IEnumerator IrRetroalimentacion()
    {
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene("Retroalimentacion");
    }

    void AplicarSkinGuardada()
    {
        int skin = GameManager.Instance != null ? GameManager.Instance.SkinSeleccionada : 0;
        if (skin < 0 || skin >= colores.Length) skin = 0;

        Color colorElegido = colores[skin];
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
            if (sr.material != null)
                sr.material.SetColor("_Color", colorElegido);

        if (numeroDisplay != null)
            numeroDisplay.CambiarColor(colorElegido);
    }
}