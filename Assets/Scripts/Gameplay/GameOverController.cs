using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// GameOverController muestra el puntaje final leído desde GameManager.
//
// SETUP en Unity:
//   El UIDocument de GameOver debe tener:
//     - Un Label llamado "LabelPuntajeFinal"  → muestra "SCORE: 0"
//     - Un Button llamado "BtnReintentar"
//     - Un Button llamado "BtnMenu"

public class GameOverController : MonoBehaviour
{
    private UIDocument ui;

    private Label  labelPuntajeFinal;
    private Button btnReintentar;
    private Button btnMenu;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        labelPuntajeFinal = root.Q<Label>("LabelPuntajeFinal");
        btnReintentar     = root.Q<Button>("BtnReintentar");
        btnMenu           = root.Q<Button>("BtnMenu");

        // Mostrar puntaje final
        if (labelPuntajeFinal != null && GameManager.Instance != null)
            labelPuntajeFinal.text = "SCORE: " + GameManager.Instance.Puntaje;

        if (btnReintentar != null)
            btnReintentar.RegisterCallback<ClickEvent>(Reintentar);

        if (btnMenu != null)
            btnMenu.RegisterCallback<ClickEvent>(IrMenu);
    }

    void OnDisable()
    {
        if (btnReintentar != null)
            btnReintentar.UnregisterCallback<ClickEvent>(Reintentar);

        if (btnMenu != null)
            btnMenu.UnregisterCallback<ClickEvent>(IrMenu);
    }

    void Reintentar(ClickEvent evt)
    {
        // Vuelve a selección de dificultad (que reinicia GameManager)
        SceneManager.LoadScene("Dificultad");
    }

    void IrMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
