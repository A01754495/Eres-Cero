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

        //  BOTONES CON SONIDO 
        if (btnFacil != null)
            btnFacil.clicked += OnFacil;

        if (btnIntermedio != null)
            btnIntermedio.clicked += OnIntermedio;

        if (btnDificil != null)
            btnDificil.clicked += OnDificil;

        if (btnVolver != null)
            btnVolver.clicked += OnVolver;
    }

    void OnDisable()
    {
        if (btnFacil != null)
            btnFacil.clicked -= OnFacil;

        if (btnIntermedio != null)
            btnIntermedio.clicked -= OnIntermedio;

        if (btnDificil != null)
            btnDificil.clicked -= OnDificil;

        if (btnVolver != null)
            btnVolver.clicked -= OnVolver;
    }

    //  FUNCIONES CON SONIDO 
    void OnFacil()
    {
        UISoundManager.Instance.PlayClick();
        IniciarJuego("facil");
    }

    void OnIntermedio()
    {
        UISoundManager.Instance.PlayClick();
        IniciarJuego("medio");
    }

    void OnDificil()
    {
        UISoundManager.Instance.PlayClick();
        IniciarJuego("dificil");
    }

    void OnVolver()
    {
        UISoundManager.Instance.PlayClick();
        VolverMenu();
    }

    void IniciarJuego(string dificultad)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarPartida(dificultad);

        SceneManager.LoadScene("Gameplay");
    }

    void VolverMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}