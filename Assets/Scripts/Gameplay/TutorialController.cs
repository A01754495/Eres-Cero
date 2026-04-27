using UnityEngine;
using UnityEngine.UIElements;

public class TutorialController : MonoBehaviour
{
    private VisualElement panelTutorial;
    private Label         labelTutorial;
    private Button        btnContinuar;

    private string[] mensajes = new string[]
    {
        "¡Bienvenido a ¡ERES CERO!\n\nTu personaje tiene un valor numérico.\nTu objetivo: igualarlo al número de la puerta.",
        "Usa ← → (o A / D) para moverte\nentre los 3 carriles.",
        "Las casillas modifican tu valor.\nElige la operación correcta\npara llegar al número de la puerta.",
        "Si llegas a la puerta con el\nnúmero correcto... ¡pasas!\nSi no, ¡Game Over!",
        "La velocidad aumenta con cada puerta.\n¡Entre más puertas cruces,\nmás puntos ganas!\n\n¿Ya sabes qué casilla elegir?\n¡Presiona ESPACIO para adelantar\ntodo y llegar más rápido a la puerta!",
        "¡Buena suerte!"
    };

    private int mensajeActual = 0;

    void Start()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EsTutorial)
        {
            gameObject.SetActive(false);
            return;
        }

        var ui = FindFirstObjectByType<GameplayController>()?.GetComponent<UIDocument>();
        if (ui == null) { gameObject.SetActive(false); return; }

        var root      = ui.rootVisualElement;
        panelTutorial = root.Q<VisualElement>("PanelTutorial");
        labelTutorial = root.Q<Label>("LabelTutorial");
        btnContinuar  = root.Q<Button>("BtnContinuar");

        if (panelTutorial == null) { gameObject.SetActive(false); return; }

        PausarJuego(true);
        MostrarMensaje(0);

        if (btnContinuar != null)
            btnContinuar.RegisterCallback<ClickEvent>(SiguienteMensaje);
    }

    void OnDisable()
    {
        if (btnContinuar != null)
            btnContinuar.UnregisterCallback<ClickEvent>(SiguienteMensaje);
    }

    void MostrarMensaje(int indice)
    {
        if (panelTutorial == null) return;
        panelTutorial.style.display = DisplayStyle.Flex;

        if (labelTutorial != null)
            labelTutorial.text = mensajes[indice];

        if (btnContinuar != null)
            btnContinuar.text = (indice == mensajes.Length - 1) ? "¡Jugar!" : "Continuar";
    }

    void SiguienteMensaje(ClickEvent evt)
    {
        mensajeActual++;

        if (mensajeActual >= mensajes.Length)
        {
            panelTutorial.style.display       = DisplayStyle.None;
            PausarJuego(false);
            GameManager.Instance.EsTutorial       = false;
            GameManager.Instance.EsPrimeraPartida = false;
        }
        else
        {
            MostrarMensaje(mensajeActual);
        }
    }

    void PausarJuego(bool pausar)
    {
        Time.timeScale = pausar ? 0f : 1f;
    }
}