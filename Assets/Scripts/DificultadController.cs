using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DificultadController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnFacil;
    private Button btnIntermedio;
    private Button btnDificil;
    private Button btnVolver;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        // BOTONES
        btnFacil = root.Q<Button>("BtnFacil");
        btnIntermedio = root.Q<Button>("BtnIntermedio");
        btnDificil = root.Q<Button>("BtnDificil");
        btnVolver = root.Q<Button>("BtnVolver");

        // CALLBACKS
        if (btnFacil != null)
            btnFacil.RegisterCallback<ClickEvent>(IrAGameplay);

        if (btnIntermedio != null)
            btnIntermedio.RegisterCallback<ClickEvent>(IrAGameplay);

        if (btnDificil != null)
            btnDificil.RegisterCallback<ClickEvent>(IrAGameplay);

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(VolverMenu);
    }

    void OnDisable()
    {
        if (btnFacil != null)
            btnFacil.UnregisterCallback<ClickEvent>(IrAGameplay);

        if (btnIntermedio != null)
            btnIntermedio.UnregisterCallback<ClickEvent>(IrAGameplay);

        if (btnDificil != null)
            btnDificil.UnregisterCallback<ClickEvent>(IrAGameplay);

        if (btnVolver != null)
            btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);
    }

    // FUNCIONES

    void IrAGameplay(ClickEvent evt)
    {
        SceneManager.LoadScene("Gameplay");
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}