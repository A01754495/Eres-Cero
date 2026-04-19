using UnityEngine;
using UnityEngine.UIElements;

// ================================================================
// GameplayController — HUD durante el juego
// Actualiza puntaje, valor del jugador y acumula tiempo de partida
// ================================================================

public class GameplayController : MonoBehaviour
{
    private UIDocument ui;
    private Label labelPuntaje;
    private Label labelValor;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;
        labelPuntaje = root.Q<Label>("LabelPuntaje");
        labelValor   = root.Q<Label>("LabelValor");
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // Acumular tiempo de partida para guardarlo en la BD al terminar
        GameManager.Instance.ActualizarTiempo();

        if (labelPuntaje != null)
            labelPuntaje.text = "PUNTAJE: " + GameManager.Instance.Puntaje;

        if (labelValor != null)
            labelValor.text = "VALOR: " + GameManager.Instance.ValorJugador;
    }
}