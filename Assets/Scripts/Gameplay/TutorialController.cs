using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

// TutorialController muestra mensajes de tutorial durante la primera partida.
//
// SETUP en Unity:
//   1. En la escena Gameplay, en el UIDocument (UIGameplay), agrega:
//      - Un VisualElement llamado "PanelTutorial" (fondo semitransparente, centrado)
//      - Un Label llamado "LabelTutorial" (texto grande, centrado dentro del panel)
//      - Un Button llamado "BtnContinuar" (dentro del panel)
//   2. Crea un GameObject vacío en Gameplay llamado "Tutorial"
//   3. Agrégale este script

public class TutorialController : MonoBehaviour
{
    private VisualElement panelTutorial;
    private Label         labelTutorial;
    private Button        btnContinuar;

    private LaneSpawner spawner;
    private PlayerController player;

    // Mensajes del tutorial en orden
    private string[] mensajes = new string[]
    {
        "¡Bienvenido a ¡ERES CERO!\n\nTu personaje tiene un valor numérico.\nTu objetivo: igualarlo al número de la puerta.",
        "Usa ← → (o A / D) para moverte\nentre los 3 carriles.",
        "Las casillas modifican tu valor.\nElige la operación correcta\npara llegar al número de la puerta.",
        "Si llegas a la puerta con el\nnúmero correcto... ¡pasas!\nSi no, ¡Game Over!",
        "La velocidad aumenta con cada puerta.\n¡Entre más puertas cruces,\nmás puntos ganas!\n¿Ya sabes qué casilla elegir?\n¡Presiona ESPACIO para adelantar\ntodo y llegar más rápido a la puerta!\n¡Buena suerte!"
    };

    private int mensajeActual = 0;

    void Start()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EsTutorial)
        {
            gameObject.SetActive(false);
            return;
        }

        // Buscar referencias en el UIDocument
        var ui   = FindFirstObjectByType<GameplayController>()?.GetComponent<UIDocument>();
        if (ui == null) { gameObject.SetActive(false); return; }

        var root        = ui.rootVisualElement;
        panelTutorial   = root.Q<VisualElement>("PanelTutorial");
        labelTutorial   = root.Q<Label>("LabelTutorial");
        btnContinuar    = root.Q<Button>("BtnContinuar");

        if (panelTutorial == null) { gameObject.SetActive(false); return; }

        spawner = FindFirstObjectByType<LaneSpawner>();
        player  = FindFirstObjectByType<PlayerController>();

        // Pausar el juego y mostrar primer mensaje
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

        // Cambiar texto del botón en el último mensaje
        if (btnContinuar != null)
            btnContinuar.text = (indice == mensajes.Length - 1) ? "¡Jugar!" : "Continuar";
    }

    void SiguienteMensaje(ClickEvent evt)
    {
        mensajeActual++;

        if (mensajeActual >= mensajes.Length)
        {
            // Tutorial terminado
            panelTutorial.style.display = DisplayStyle.None;
            PausarJuego(false);

            // Marcar que ya jugó para que no vuelva a ver el tutorial
            GameManager.Instance.MarcarPartidaJugada();
            GameManager.Instance.EsTutorial = false;
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