using UnityEngine;
using UnityEngine.UIElements;

public class GameplayController : MonoBehaviour
{
    private UIDocument ui;
    private Label labelPuntaje;
    private Label labelValor;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return; // si no hay UIDocument, no hace nada

        var root = ui.rootVisualElement;
        if (root == null) return;

        labelPuntaje = root.Q<Label>("LabelPuntaje");
        labelValor   = root.Q<Label>("LabelValor");
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (labelPuntaje != null)
            labelPuntaje.text = "PUNTAJE: " + GameManager.Instance.Puntaje;

        if (labelValor != null)
            labelValor.text = "VALOR: " + GameManager.Instance.ValorJugador;
    }
}