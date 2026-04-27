using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

// Verifica Logro 5 — Dominio absoluto: top 1 en ranking historico

public class MenuController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

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

        // Verificar Dominio absoluto si hay sesión
        if (GameManager.Instance != null && GameManager.Instance.HaySesion)
            StartCoroutine(VerificarDominioAbsoluto());
    }

    void OnDisable() { }

    // ================================================================
    // Logro 5 — Dominio absoluto: top 1 en ranking historico
    // ================================================================
    IEnumerator VerificarDominioAbsoluto()
    {
        var gm = GameManager.Instance;

        // Verificar si ya está desbloqueado en BD
        using var check = UnityWebRequest.Get($"{URL_BASE}/logros-jugador/{gm.IdJugador}");
        yield return check.SendWebRequest();

        if (check.result == UnityWebRequest.Result.Success)
        {
            var lista = JsonHelper.ParseList<LogroEntry>(check.downloadHandler.text);
            foreach (var l in lista)
                if (l.idLogro == 5) yield break; // ya desbloqueado
        }

        // Consultar ranking historico
        using var req = UnityWebRequest.Get($"{URL_BASE}/ranking-historico");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        var ranking = JsonHelper.ParseList<RankingEntry>(req.downloadHandler.text);
        if (ranking == null || ranking.Count == 0) yield break;

        // El top 1 es el primer elemento — comparar alias
        if (ranking[0].alias == gm.AliasJugador)
            yield return StartCoroutine(DesbloquearLogro(5));
    }

    IEnumerator DesbloquearLogro(int idLogro)
    {
        string fecha = System.DateTime.Now.ToString("yyyy-MM-dd");
        string body  = $"{{\"idJugador\":{GameManager.Instance.IdJugador}," +
                       $"\"idLogro\":{idLogro}," +
                       $"\"fechaDesbloqueo\":\"{fecha}\"}}";

        using var req = new UnityWebRequest($"{URL_BASE}/logro", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogWarning($"No se pudo desbloquear logro {idLogro}: " + req.downloadHandler.text);
        else
            Debug.Log($"Logro {idLogro} desbloqueado!");
    }

    void IrAJugar(ClickEvent evt)
    {
        if (GameManager.Instance != null && GameManager.Instance.EsPrimeraPartida)
        {
            GameManager.Instance.EsTutorial = true;
            GameManager.Instance.IniciarPartida("facil");
            SceneManager.LoadScene("Gameplay"); // <- directo a Gameplay, el TutorialController maneja el video
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

    [System.Serializable] private class LogroEntry  { public int idLogro; }
    [System.Serializable] private class RankingEntry { public int posicion; public string alias; public int puntaje; }
}
