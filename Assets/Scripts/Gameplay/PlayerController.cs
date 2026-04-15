using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Carriles (posición X de cada uno)")]
    public float carrilIzquierda = -2.5f;
    public float carrilCentro   =  0f;
    public float carrilDerecha  =  2.5f;

    [Header("Velocidad de transición entre carriles")]
    public float velocidadMovimiento = 12f;

    [Header("UI: texto que muestra el valor del jugador")]
    public TextMeshPro textoValor;

    private int carrilActual = 1;
    private float xObjetivo;
    private bool puedeMover = true;

    private InputAction moverIzquierda;
    private InputAction moverDerecha;
    public bool puedeCambiarCarril = true;

    void Awake()
    {
        moverIzquierda = new InputAction(type: InputActionType.Button);
        moverIzquierda.AddBinding("<Keyboard>/leftArrow");
        moverIzquierda.AddBinding("<Keyboard>/a");

        moverDerecha = new InputAction(type: InputActionType.Button);
        moverDerecha.AddBinding("<Keyboard>/rightArrow");
        moverDerecha.AddBinding("<Keyboard>/d");
    }

    void OnEnable()
    {
        moverIzquierda.Enable();
        moverDerecha.Enable();
        moverIzquierda.performed += OnMoverIzquierda;
        moverDerecha.performed += OnMoverDerecha;
    }

    void OnDisable()
    {
        moverIzquierda.performed -= OnMoverIzquierda;
        moverDerecha.performed -= OnMoverDerecha;
        moverIzquierda.Disable();
        moverDerecha.Disable();
    }

    void OnMoverIzquierda(InputAction.CallbackContext ctx) => CambiarCarril(-1);
    void OnMoverDerecha(InputAction.CallbackContext ctx) => CambiarCarril(1);

    void Start()
    {
        carrilActual = 1;
        xObjetivo = carrilCentro;
        ActualizarTexto();
    }

    void Update()
    {
        if (!puedeMover) return;
        MoverHaciaCarril();
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
        if (other.CompareTag("Operacion"))
        {
            if (!puedeMover) return;

            CasillaOperacion casilla = other.GetComponent<CasillaOperacion>();
            if (casilla == null) return;

            GameManager.Instance.ValorJugador = casilla.resultadoFinal;
            ActualizarTexto();
            puedeCambiarCarril = false;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Puerta"))
        {
            PuertaController puerta = other.GetComponent<PuertaController>();
            if (puerta == null) return;

            if (GameManager.Instance.ValorJugador == puerta.numeroMeta)
            {
                GameManager.Instance.Puntaje += 100 + GameManager.Instance.PuertasVivas * 10;
                GameManager.Instance.PuertasVivas += 1;
                GameManager.Instance.ValorJugador = puerta.numeroMeta;
                GameManager.Instance.ValorBase = puerta.numeroMeta;

                puerta.MostrarExito();
                ActualizarTexto();

                puedeCambiarCarril = true;

                FindFirstObjectByType<LaneSpawner>()?.ForzarNuevaOla();
            }
            else
            {
                puedeMover = false;
                puerta.MostrarError();
                Invoke(nameof(IrGameOver), 0.8f);
            }
        }
    }

    void ActualizarTexto()
    {
        if (textoValor != null)
            textoValor.text = GameManager.Instance.ValorJugador.ToString();
    }

    void IrGameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}