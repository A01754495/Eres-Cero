using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// CU-06: Muestra logros del jugador
// Excepción CU-06: si no ha jugado ninguna partida, muestra mensaje de "sin datos"

public class LogrosController : MonoBehaviour
{
    private UIDocument ui;
    private Button        btnVolver;
    private VisualElement contenedorLogros;
    private Label         labelSinDatos;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnVolver        = root.Q<Button>("BtnVolver");
        contenedorLogros = root.Q<VisualElement>("ContenedorLogros");

        labelSinDatos = root.Q<Label>("LabelSinDatos");

        //  BOTÓN VOLVER CON SONIDO 
        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                VolverMenu(e);
            });

        VerificarPartidas();
    }

    void OnDisable()
    {
        //  No desregistramos por uso de lambda (no pasa nada en UI Toolkit) 
    }

    void VerificarPartidas()
    {
        bool haJugado = PlayerPrefs.GetInt("HaJugado", 0) == 1;

        if (!haJugado)
        {
            if (contenedorLogros != null)
                contenedorLogros.style.display = DisplayStyle.None;

            if (labelSinDatos != null)
                labelSinDatos.style.display = DisplayStyle.Flex;
        }
        else
        {
            if (contenedorLogros != null)
                contenedorLogros.style.display = DisplayStyle.Flex;

            if (labelSinDatos != null)
                labelSinDatos.style.display = DisplayStyle.None;
        }
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}