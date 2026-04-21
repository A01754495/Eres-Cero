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

        //  BOTÓN VOLVER CON SONIDO 
        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                VolverMenu(e);
            });

        //  BOTÓN CERRAR SESIÓN CON SONIDO 
        if (btnCerrarSesion != null)
            btnCerrarSesion.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                CerrarSesion(e);
            });
    }

    void OnDisable()
    {
        //  No se desregistran porque usamos lambdas 
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    void CerrarSesion(ClickEvent evt)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.CerrarSesion();

        SceneManager.LoadScene("Login");
    }
}