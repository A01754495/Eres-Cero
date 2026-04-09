using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    private UIDocument ui;

    // PANELES
    private VisualElement panelLogin;
    private VisualElement panelRegistro;

    // BOTONES CAMBIO PANEL
    private Button btnIrRegistro;
    private Button btnIrLogin;

    // BOTONES ACCIÓN
    private Button btnLogin;
    private Button btnRegistro;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        // PANELES
        panelLogin = root.Q<VisualElement>("PanelLogin");
        panelRegistro = root.Q<VisualElement>("PanelRegistro");

        // BOTONES CAMBIO
        btnIrRegistro = root.Q<Button>("BtnIrRegistro");
        btnIrLogin = root.Q<Button>("BtnIrLogin");

        // BOTONES ACCIÓN 
        btnLogin = root.Q<Button>("BtnLogin");
        btnRegistro = root.Q<Button>("BtnRegistro");

        // ESTADO INICIAL
        panelLogin.style.display = DisplayStyle.Flex;
        panelRegistro.style.display = DisplayStyle.None;

        // CAMBIO DE PANEL
        if (btnIrRegistro != null)
            btnIrRegistro.RegisterCallback<ClickEvent>(MostrarRegistro);

        if (btnIrLogin != null)
            btnIrLogin.RegisterCallback<ClickEvent>(MostrarLogin);

        // ACCIONES 
        if (btnLogin != null)
            btnLogin.RegisterCallback<ClickEvent>(IrAMenu);

        if (btnRegistro != null)
            btnRegistro.RegisterCallback<ClickEvent>(IrAMenu);
    }

    void OnDisable()
    {
        if (btnIrRegistro != null)
            btnIrRegistro.UnregisterCallback<ClickEvent>(MostrarRegistro);

        if (btnIrLogin != null)
            btnIrLogin.UnregisterCallback<ClickEvent>(MostrarLogin);

        if (btnLogin != null)
            btnLogin.UnregisterCallback<ClickEvent>(IrAMenu);

        if (btnRegistro != null)
            btnRegistro.UnregisterCallback<ClickEvent>(IrAMenu);
    }

    // CAMBIOS DE PANEL

    void MostrarRegistro(ClickEvent evt)
    {
        panelLogin.style.display = DisplayStyle.None;
        panelRegistro.style.display = DisplayStyle.Flex;
    }

    void MostrarLogin(ClickEvent evt)
    {
        panelLogin.style.display = DisplayStyle.Flex;
        panelRegistro.style.display = DisplayStyle.None;
    }

    //  IR AL MENÚ

    void IrAMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}