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
        if (ui == null) return;

        var root = ui.rootVisualElement;

        btnFacil      = root.Q<Button>("BtnFacil");
        btnIntermedio = root.Q<Button>("BtnIntermedio");
        btnDificil    = root.Q<Button>("BtnDificil");
        btnVolver     = root.Q<Button>("BtnVolver");

        // Usamos .clicked en lugar de RegisterCallback para simplicidad
        if (btnFacil      != null) btnFacil.clicked += () => IniciarJuego("facil");
        if (btnIntermedio != null) btnIntermedio.clicked += () => IniciarJuego("medio");
        if (btnDificil    != null) btnDificil.clicked += () => IniciarJuego("dificil");
        if (btnVolver     != null) btnVolver.clicked += VolverMenu;
    }

    void OnDisable()
    {
        // Es vital desuscribirse con el mismo formato
        if (btnFacil      != null) btnFacil.clicked -= () => IniciarJuego("facil");
        if (btnIntermedio != null) btnIntermedio.clicked -= () => IniciarJuego("medio");
        if (btnDificil    != null) btnDificil.clicked -= () => IniciarJuego("dificil");
        if (btnVolver     != null) btnVolver.clicked -= VolverMenu;
    }

    void IniciarJuego(string dificultad)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarPartida(dificultad);

        SceneManager.LoadScene("Gameplay");
    }

    void VolverMenu() // Quitamos el ClickEvent evt para que sea compatible con .clicked
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}