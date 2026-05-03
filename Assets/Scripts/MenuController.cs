using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
<<<<<<< HEAD
using UnityEngine.Networking;
using System.Collections;
=======
>>>>>>> 693feae (Final)

public class MenuController : MonoBehaviour
{
    private const string URL_BASE =
        "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument menu;
    private Button btnJugar;
    private Button btnRankings;
    private Button btnAspectos;
    private Button btnCreditos;
    private VisualElement iconoTrofeo;
    private VisualElement iconoUsuario;

    void OnEnable()
    {
        menu = GetComponent<UIDocument>();
        var root = menu.rootVisualElement;

        btnJugar = root.Q<Button>("BtnJugar");
        btnRankings = root.Q<Button>("BtnRankings");
        btnAspectos = root.Q<Button>("BtnAspectos");
        btnCreditos = root.Q<Button>("BtnCreditos");
        iconoTrofeo = root.Q<VisualElement>("IconoTrofeo");
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

        if (btnCreditos != null)
            btnCreditos.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                SceneManager.LoadScene("Creditos");
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

        if (GameManager.Instance != null && GameManager.Instance.HaySesion)
        {
            // Deshabilitar Jugar hasta que sepa si es primera partida
<<<<<<< HEAD
            if (btnJugar != null) btnJugar.SetEnabled(false);
=======
            if (btnJugar != null)
                btnJugar.SetEnabled(false);
>>>>>>> 693feae (Final)
            StartCoroutine(VerificarPrimeraPartidaAlIniciar());
            StartCoroutine(VerificarDominioAbsoluto());
        }
    }

    void OnDisable() { }

    IEnumerator VerificarPrimeraPartidaAlIniciar()
    {
        int idJugador = GameManager.Instance.IdJugador;
        using var req = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<RespuestaPuntaje>(req.downloadHandler.text);
            GameManager.Instance.EsPrimeraPartida = (resp != null && resp.puntajeTotal == 0);
        }

        // Habilitar Jugar una vez que ya sabe si es primera partida
<<<<<<< HEAD
        if (btnJugar != null) btnJugar.SetEnabled(true);
=======
        if (btnJugar != null)
            btnJugar.SetEnabled(true);
>>>>>>> 693feae (Final)
    }

    IEnumerator VerificarDominioAbsoluto()
    {
        var gm = GameManager.Instance;

        using var check = UnityWebRequest.Get($"{URL_BASE}/logros-jugador/{gm.IdJugador}");
        yield return check.SendWebRequest();

        if (check.result == UnityWebRequest.Result.Success)
        {
            var lista = JsonHelper.ParseList<LogroEntry>(check.downloadHandler.text);
            foreach (var l in lista)
                if (l.idLogro == 5)
                    yield break;
        }

        using var req = UnityWebRequest.Get($"{URL_BASE}/ranking-historico");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            yield break;

        var ranking = JsonHelper.ParseList<RankingEntry>(req.downloadHandler.text);
        if (ranking == null || ranking.Count == 0)
            yield break;

        if (ranking[0].alias == gm.AliasJugador)
            yield return StartCoroutine(DesbloquearLogro(5));
    }

    IEnumerator DesbloquearLogro(int idLogro)
    {
        string fecha = System.DateTime.Now.ToString("yyyy-MM-dd");
        string body =
            $"{{\"idJugador\":{GameManager.Instance.IdJugador},"
            + $"\"idLogro\":{idLogro},"
            + $"\"fechaDesbloqueo\":\"{fecha}\"}}";

        using var req = new UnityWebRequest($"{URL_BASE}/logro", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogWarning(
                $"No se pudo desbloquear logro {idLogro}: " + req.downloadHandler.text
            );
        else
            Debug.Log($"Logro {idLogro} desbloqueado!");
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

<<<<<<< HEAD
    [System.Serializable] private class LogroEntry       { public int idLogro; }
    [System.Serializable] private class RankingEntry     { public int posicion; public string alias; public int puntaje; }
    [System.Serializable] private class RespuestaPuntaje { public int puntajeTotal; }
}
=======
    void IrAAspectos(ClickEvent evt) => SceneManager.LoadScene("Aspectos");

    void IrALogros(ClickEvent evt) => SceneManager.LoadScene("Logros");

    void IrALogin(ClickEvent evt) => SceneManager.LoadScene("Perfil");

    [System.Serializable]
    private class LogroEntry
    {
        public int idLogro;
    }

    [System.Serializable]
    private class RankingEntry
    {
        public int posicion;
        public string alias;
        public int puntaje;
    }

    [System.Serializable]
    private class RespuestaPuntaje
    {
        public int puntajeTotal;
    }
}
>>>>>>> 693feae (Final)
