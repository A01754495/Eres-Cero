using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private UIDocument menu;

    // BOTONES
    private Button btnJugar;
    private Button btnRankings;
    private Button btnAspectos;

    // ICONOS (NO son Button)
    private VisualElement iconoTrofeo;
    private VisualElement iconoUsuario;

    void OnEnable()
    {
        menu = GetComponent<UIDocument>();
        var root = menu.rootVisualElement;

        // BOTONES
        btnJugar = root.Q<Button>("BtnJugar");
        btnRankings = root.Q<Button>("BtnRankings");
        btnAspectos = root.Q<Button>("BtnAspectos");

        // ICONOS
        iconoTrofeo = root.Q<VisualElement>("IconoTrofeo");
        iconoUsuario = root.Q<VisualElement>("IconoUsuario");

        // CALLBACKS BOTONES
        if (btnJugar != null)
            btnJugar.RegisterCallback<ClickEvent>(IrAJugar);

        if (btnRankings != null)
            btnRankings.RegisterCallback<ClickEvent>(IrARankings);

        if (btnAspectos != null)
            btnAspectos.RegisterCallback<ClickEvent>(IrAAspectos);

        // CALLBACKS ICONOS
        if (iconoTrofeo != null)
            iconoTrofeo.RegisterCallback<ClickEvent>(IrALogros);

        if (iconoUsuario != null)
            iconoUsuario.RegisterCallback<ClickEvent>(IrALogin);
    }

    void OnDisable()
    {
        if (btnJugar != null)
            btnJugar.UnregisterCallback<ClickEvent>(IrAJugar);

        if (btnRankings != null)
            btnRankings.UnregisterCallback<ClickEvent>(IrARankings);

        if (btnAspectos != null)
            btnAspectos.UnregisterCallback<ClickEvent>(IrAAspectos);

        if (iconoTrofeo != null)
            iconoTrofeo.UnregisterCallback<ClickEvent>(IrALogros);

        if (iconoUsuario != null)
            iconoUsuario.UnregisterCallback<ClickEvent>(IrALogin);
    }

    // FUNCIONES

    void IrAJugar(ClickEvent evt)
    {
        SceneManager.LoadScene("Dificultad");
    }

    void IrARankings(ClickEvent evt)
    {
        SceneManager.LoadScene("Ranking");
    }

    void IrAAspectos(ClickEvent evt)
    {
        SceneManager.LoadScene("Aspectos");
    }

    void IrALogros(ClickEvent evt)
    {
        SceneManager.LoadScene("Logros");
    }

    void IrALogin(ClickEvent evt)
    {
        SceneManager.LoadScene("Perfil");
    }
}