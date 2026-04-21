using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private UIDocument menu;

    private Button btnJugar;
    private Button btnRankings;
    private Button btnAspectos;

    private VisualElement iconoTrofeo;
    private VisualElement iconoUsuario;

    void OnEnable()
    {
        menu = GetComponent<UIDocument>();
        var root = menu.rootVisualElement;

        btnJugar    = root.Q<Button>("BtnJugar");
        btnRankings = root.Q<Button>("BtnRankings");
        btnAspectos = root.Q<Button>("BtnAspectos");

        iconoTrofeo  = root.Q<VisualElement>("IconoTrofeo");
        iconoUsuario = root.Q<VisualElement>("IconoUsuario");

        //  agregamos sonido
        if (btnJugar != null)
            btnJugar.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                IrAJugar(e);
            });

        if (btnRankings != null)
            btnRankings.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                IrARankings(e);
            });

        if (btnAspectos != null)
            btnAspectos.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                IrAAspectos(e);
            });

        if (iconoTrofeo != null)
            iconoTrofeo.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                IrALogros(e);
            });

        if (iconoUsuario != null)
            iconoUsuario.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                IrALogin(e);
            });
    }

    void OnDisable()
    {
        //  Estos no se pueden quitar fácilmente porque usamos lambdas 
    }

    void IrAJugar(ClickEvent evt)
    {
        if (GameManager.Instance != null && GameManager.Instance.EsPrimeraPartida)
        {
            GameManager.Instance.EsTutorial = true;
            GameManager.Instance.IniciarPartida("facil");
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            GameManager.Instance.EsTutorial = false;
            SceneManager.LoadScene("Dificultad");
        }
    }

    void IrARankings(ClickEvent evt) => SceneManager.LoadScene("Ranking");
    void IrAAspectos(ClickEvent evt) => SceneManager.LoadScene("Aspectos");
    void IrALogros(ClickEvent evt)   => SceneManager.LoadScene("Logros");
    void IrALogin(ClickEvent evt)    => SceneManager.LoadScene("Perfil");
}