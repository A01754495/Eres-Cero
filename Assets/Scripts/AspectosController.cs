using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// CU-07: Aspectos/Skins
// - Skin 1 siempre desbloqueada
// - Las demás se desbloquean por puntaje acumulado (guardado en PlayerPrefs)
// - El jugador selecciona una y se mantiene hasta cerrar el programa
// - La skin seleccionada se guarda en GameManager.SkinSeleccionada

public class AspectosController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnSeleccionar;
    private Button btnSeleccionado; // botón que aparece cuando ya está seleccionada
    private Button btnVolver;
    private VisualElement imagenPreview;

    // Botones de skin del grid
    private Button[] btnSkins = new Button[12];

    // Puntaje necesario para desbloquear cada skin (índice 0 = skin 1)
    private readonly int[] puntajesDesbloqueo = {
        0,    // Skin 1 — siempre desbloqueada
        100,  // Skin 2
        5000,  // Skin 3
        10000,  // Skin 4
        15000,  // Skin 5
        20000, // Skin 6
        25000, // Skin 7
        30000, // Skin 8
        35000, // Skin 9
        40000, // Skin 10
        45000, // Skin 11
        100000  // Skin 12
    };

    private int skinSeleccionadaActual = 0; // índice 0-based
    private int skinEnPreview          = 0;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnSeleccionar  = root.Q<Button>("BtnSeleccionar");
        btnSeleccionado = root.Q<Button>("BtnSeleccionado");
        btnVolver       = root.Q<Button>("BtnVolver");
        imagenPreview   = root.Q<VisualElement>("ImagenPreview");

        // Cargar los 12 botones de skin
        for (int i = 0; i < 12; i++)
            btnSkins[i] = root.Q<Button>($"BtnSkin{i + 1}");

        // Cargar skin seleccionada guardada
        skinSeleccionadaActual = PlayerPrefs.GetInt("SkinSeleccionada", 0);
        skinEnPreview          = skinSeleccionadaActual;

        // Puntaje acumulado histórico
        int puntajeTotal = PlayerPrefs.GetInt("PuntajeTotal", 0);

        // Actualizar puntaje total si la sesión actual tiene más
        if (GameManager.Instance != null)
        {
            int puntajeSesion = GameManager.Instance.Puntaje;
            if (puntajeSesion > 0)
            {
                puntajeTotal += puntajeSesion;
                PlayerPrefs.SetInt("PuntajeTotal", puntajeTotal);
                PlayerPrefs.Save();
            }
        }

        // Aplicar estado bloqueado/desbloqueado a cada botón
        AplicarEstadosSkins(puntajeTotal);

        // Preview de la skin actualmente seleccionada
        ActualizarPreview(skinSeleccionadaActual);

        // Callbacks de los botones de skin
        for (int i = 0; i < 12; i++)
        {
            int idx = i; // capturar para el closure
            if (btnSkins[idx] != null)
                btnSkins[idx].RegisterCallback<ClickEvent>(_ => OnClickSkin(idx));
        }

        if (btnSeleccionar  != null) btnSeleccionar.RegisterCallback<ClickEvent>(OnSeleccionar);
        if (btnVolver       != null) btnVolver.RegisterCallback<ClickEvent>(OnVolver);
    }

    void OnDisable()
    {
        for (int i = 0; i < 12; i++)
        {
            int idx = i;
            if (btnSkins[idx] != null)
                btnSkins[idx].UnregisterCallback<ClickEvent>(_ => OnClickSkin(idx));
        }
        if (btnSeleccionar != null) btnSeleccionar.UnregisterCallback<ClickEvent>(OnSeleccionar);
        if (btnVolver      != null) btnVolver.UnregisterCallback<ClickEvent>(OnVolver);
    }

    // ----------------------------------------------------------------
    // Aplica overlay oscuro a las skins bloqueadas y quita el de las desbloqueadas
    // ----------------------------------------------------------------
    void AplicarEstadosSkins(int puntajeTotal)
    {
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;

            bool desbloqueada = puntajeTotal >= puntajesDesbloqueo[i];

            if (desbloqueada)
            {
                // Sin overlay — apariencia normal
                btnSkins[i].style.opacity = 1f;
                btnSkins[i].SetEnabled(true);
                btnSkins[i].tooltip = "";
            }
            else
            {
                // Oscurecer botón para indicar que está bloqueado
                btnSkins[i].style.opacity = 0.35f;
                btnSkins[i].SetEnabled(false);
                btnSkins[i].tooltip = $"Necesitas {puntajesDesbloqueo[i]} puntos para desbloquear";
            }
        }
    }

    // ----------------------------------------------------------------
    // Al hacer click en una skin — mostrar en preview
    // ----------------------------------------------------------------
    void OnClickSkin(int idx)
    {
        skinEnPreview = idx;
        ActualizarPreview(idx);
    }

    // ----------------------------------------------------------------
    // Actualizar imagen de preview y estado del botón Seleccionar
    // ----------------------------------------------------------------
    void ActualizarPreview(int idx)
    {
        // Resaltar el botón seleccionado en el grid
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;
            btnSkins[i].style.borderTopWidth    = (i == idx) ? 3 : 0;
            btnSkins[i].style.borderBottomWidth = (i == idx) ? 3 : 0;
            btnSkins[i].style.borderLeftWidth   = (i == idx) ? 3 : 0;
            btnSkins[i].style.borderRightWidth  = (i == idx) ? 3 : 0;
            btnSkins[i].style.borderTopColor    = new StyleColor(Color.cyan);
            btnSkins[i].style.borderBottomColor = new StyleColor(Color.cyan);
            btnSkins[i].style.borderLeftColor   = new StyleColor(Color.cyan);
            btnSkins[i].style.borderRightColor  = new StyleColor(Color.cyan);
        }

        // Mostrar/ocultar botón Seleccionar vs Seleccionado
        bool yaEsLaActual = (idx == skinSeleccionadaActual);
        if (btnSeleccionar  != null) btnSeleccionar.style.display  = yaEsLaActual ? DisplayStyle.None : DisplayStyle.Flex;
        if (btnSeleccionado != null) btnSeleccionado.style.display = yaEsLaActual ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // ----------------------------------------------------------------
    // Confirmar selección de skin
    // ----------------------------------------------------------------
    void OnSeleccionar(ClickEvent e)
    {
        skinSeleccionadaActual = skinEnPreview;

        // Guardar en PlayerPrefs y en GameManager
        PlayerPrefs.SetInt("SkinSeleccionada", skinSeleccionadaActual);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
            GameManager.Instance.SkinSeleccionada = skinSeleccionadaActual;

        // Actualizar UI
        ActualizarPreview(skinSeleccionadaActual);
    }

    void OnVolver(ClickEvent e) => SceneManager.LoadScene("MenuPrincipal");
}