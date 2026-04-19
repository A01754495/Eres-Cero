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

        // Buscar o crear el label de "sin datos"
        labelSinDatos = root.Q<Label>("LabelSinDatos");

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(VolverMenu);

        VerificarPartidas();
    }

    void OnDisable()
    {
        if (btnVolver != null)
            btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);
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

    void VolverMenu(ClickEvent evt) => SceneManager.LoadScene("MenuPrincipal");
}