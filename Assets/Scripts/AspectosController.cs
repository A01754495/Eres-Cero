using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AspectosController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnSeleccionar;
    private Button btnSeleccionado;
    private Button btnVolver;
    private VisualElement imagenPreview;

    private Button[] btnSkins = new Button[12];

    private readonly int[] puntajesDesbloqueo = {
        0, 100, 5000, 10000, 15000, 20000,
        25000, 30000, 35000, 40000, 45000, 100000
    };

    private int skinSeleccionadaActual = 0;
    private int skinEnPreview = 0;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnSeleccionar  = root.Q<Button>("BtnSeleccionar");
        btnSeleccionado = root.Q<Button>("BtnSeleccionado");
        btnVolver       = root.Q<Button>("BtnVolver");
        imagenPreview   = root.Q<VisualElement>("ImagenPreview");

        for (int i = 0; i < 12; i++)
            btnSkins[i] = root.Q<Button>($"BtnSkin{i + 1}");

        skinSeleccionadaActual = PlayerPrefs.GetInt("SkinSeleccionada", 0);
        skinEnPreview          = skinSeleccionadaActual;

        int puntajeTotal = PlayerPrefs.GetInt("PuntajeTotal", 0);

        AplicarEstadosSkins(puntajeTotal);
        ActualizarPreview(skinSeleccionadaActual);

        //  BOTONES DE SKINS 
        for (int i = 0; i < 12; i++)
        {
            int idx = i;

            if (btnSkins[idx] != null)
                btnSkins[idx].RegisterCallback<ClickEvent>(e =>
                {
                    UISoundManager.Instance.PlayClick(); // 🔊 sonido
                    OnClickSkin(idx);
                });
        }

        //  BOTÓN SELECCIONAR 
        if (btnSeleccionar != null)
            btnSeleccionar.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnSeleccionar(e);
            });

        //  BOTÓN VOLVER 
        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnVolver(e);
            });
    }

    void OnDisable()
    {
        //  No desregistramos porque usamos lambdas 
    }

    void AplicarEstadosSkins(int puntajeTotal)
    {
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;

            bool desbloqueada = puntajeTotal >= puntajesDesbloqueo[i];

            if (desbloqueada)
            {
                btnSkins[i].style.opacity = 1f;
                btnSkins[i].SetEnabled(true);
                btnSkins[i].tooltip = "";
            }
            else
            {
                btnSkins[i].style.opacity = 0.35f;
                btnSkins[i].SetEnabled(false);
                btnSkins[i].tooltip =
                    $"Necesitas {puntajesDesbloqueo[i]} puntos para desbloquear";
            }
        }
    }

    void OnClickSkin(int idx)
    {
        skinEnPreview = idx;
        ActualizarPreview(idx);
    }

    void ActualizarPreview(int idx)
    {
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;

            bool selected = (i == idx);

            btnSkins[i].style.borderTopWidth =
            btnSkins[i].style.borderBottomWidth =
            btnSkins[i].style.borderLeftWidth =
            btnSkins[i].style.borderRightWidth = selected ? 3 : 0;

            var color = new StyleColor(Color.cyan);

            btnSkins[i].style.borderTopColor =
            btnSkins[i].style.borderBottomColor =
            btnSkins[i].style.borderLeftColor =
            btnSkins[i].style.borderRightColor = color;
        }

        bool yaEsLaActual = (idx == skinSeleccionadaActual);

        if (btnSeleccionar != null)
            btnSeleccionar.style.display =
                yaEsLaActual ? DisplayStyle.None : DisplayStyle.Flex;

        if (btnSeleccionado != null)
            btnSeleccionado.style.display =
                yaEsLaActual ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void OnSeleccionar(ClickEvent e)
    {
        skinSeleccionadaActual = skinEnPreview;

        PlayerPrefs.SetInt("SkinSeleccionada", skinSeleccionadaActual);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
            GameManager.Instance.SkinSeleccionada = skinSeleccionadaActual;

        ActualizarPreview(skinSeleccionadaActual);
    }

    void OnVolver(ClickEvent e)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}