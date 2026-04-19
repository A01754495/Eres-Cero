using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PerfilController : MonoBehaviour
{
    private UIDocument ui;

    private Label  labelUsuario;
    private Button btnVolver;
    private Button btnCerrarSesion;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        labelUsuario    = root.Q<Label>("Usuario");
        btnVolver       = root.Q<Button>("BtnVolver");
        btnCerrarSesion = root.Q<Button>("BtnCerrarSesion");

        // Mostrar alias del jugador en sesión
        if (labelUsuario != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.HaySesion)
                labelUsuario.text = "Nombre de usuario: " + GameManager.Instance.AliasJugador;
            else
                labelUsuario.text = "Nombre de usuario: Invitado";
        }

        if (btnVolver       != null) btnVolver.RegisterCallback<ClickEvent>(VolverMenu);
        if (btnCerrarSesion != null) btnCerrarSesion.RegisterCallback<ClickEvent>(CerrarSesion);
    }

    void OnDisable()
    {
        if (btnVolver       != null) btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);
        if (btnCerrarSesion != null) btnCerrarSesion.UnregisterCallback<ClickEvent>(CerrarSesion);
    }

    void VolverMenu(ClickEvent evt) => SceneManager.LoadScene("MenuPrincipal");

    void CerrarSesion(ClickEvent evt)
    {
        // Limpiar sesión del GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.CerrarSesion();

        SceneManager.LoadScene("Login");
    }
}