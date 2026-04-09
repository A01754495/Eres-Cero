using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PerfilController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnVolver;
    private Button btnCerrarSesion;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        // BOTONES
        btnVolver = root.Q<Button>("BtnVolver");
        btnCerrarSesion = root.Q<Button>("BtnCerrarSesion");

        // CALLBACKS
        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(VolverMenu);

        if (btnCerrarSesion != null)
            btnCerrarSesion.RegisterCallback<ClickEvent>(CerrarSesion);
    }

    void OnDisable()
    {
        if (btnVolver != null)
            btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);

        if (btnCerrarSesion != null)
            btnCerrarSesion.UnregisterCallback<ClickEvent>(CerrarSesion);
    }

    // VOLVER AL MENÚ
    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    // 🔐 CERRAR SESIÓN
    void CerrarSesion(ClickEvent evt)
    {
        // borrar datos de sesión (opcional)
        PlayerPrefs.DeleteAll();

        // ir a login (SIEMPRE empieza en PanelLogin)
        SceneManager.LoadScene("Login");
    }
}