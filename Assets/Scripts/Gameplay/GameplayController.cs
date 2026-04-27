using UnityEngine;
using UnityEngine.UIElements;

public class GameplayController : MonoBehaviour
{
    private UIDocument ui;
    private Label labelPuntaje;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;
        labelPuntaje = root.Q<Label>("LabelPuntaje");
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ActualizarTiempo();

        if (labelPuntaje != null)
            labelPuntaje.text = "PUNTAJE: " + GameManager.Instance.Puntaje;
    }
}