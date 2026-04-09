using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnReintentar;
    private Button btnMenu;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        // BOTONES
        btnReintentar = root.Q<Button>("BtnReintentar");
        btnMenu = root.Q<Button>("BtnMenu");

        // CALLBACKS
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

    // FUNCIONES

    void Reintentar(ClickEvent evt)
    {
        SceneManager.LoadScene("Dificultad");
    }

    void IrMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}