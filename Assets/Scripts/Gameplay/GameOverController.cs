using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

// IDs de logros en BD:
// 1 = Hardcore protocol   (20 puertas en dificil en una partida)
// 2 = Identidad completa  (todos los aspectos desbloqueados)
// 3 = Ambicion creciente  (10000 pts en una partida)
// 4 = Travesia infinita   (10 min en una partida)
// 5 = Dominio absoluto    (top 1 historico)
// 6 = Overflow            (1,000,000 pts acumulados)

public class GameOverController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument ui;
    private Label  labelPuntajeFinal;
    private Label  labelPuertas;
    private Label  labelDificultad;
    private Label  labelTiempo;
    private Button btnReintentar;
    private Button btnMenu;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        labelPuntajeFinal = root.Q<Label>("LabelPuntajeFinal");
        labelPuertas      = root.Q<Label>("LabelPuertas");
        labelDificultad   = root.Q<Label>("LabelDificultad");
        labelTiempo       = root.Q<Label>("LabelTiempo");
        btnReintentar     = root.Q<Button>("BtnReintentar");
        btnMenu           = root.Q<Button>("BtnMenu");

        if (GameManager.Instance != null)
        {
            if (labelPuntajeFinal != null)
                labelPuntajeFinal.text = "PUNTAJE: " + GameManager.Instance.Puntaje;
            if (labelPuertas != null)
                labelPuertas.text = "Puertas cruzadas: " + GameManager.Instance.PuertasVivas;
            if (labelDificultad != null)
                labelDificultad.text = "Dificultad: " + GameManager.Instance.Dificultad.ToUpper();
            if (labelTiempo != null)
            {
                int seg = Mathf.FloorToInt(GameManager.Instance.TiempoPartida);
                labelTiempo.text = $"Tiempo: {seg / 60:00}:{seg % 60:00}";
            }

            if (GameManager.Instance.HaySesion)
            {
                StartCoroutine(GuardarPartida());
                StartCoroutine(VerificarLogros());
            }
        }

        if (btnReintentar != null)
            btnReintentar.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnReintentar(e);
            });

        if (btnMenu != null)
            btnMenu.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnMenu(e);
            });
    }

    void OnDisable() { }

    void OnReintentar(ClickEvent e)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarPartida(GameManager.Instance.Dificultad);
        SceneManager.LoadScene("Dificultad");
    }

    void OnMenu(ClickEvent e) => SceneManager.LoadScene("MenuPrincipal");

    // ================================================================
    // Guardar partida en BD
    // POST /partida — { idJugador, fechaHora, puntaje, dificultad, tiempo }
    // ================================================================
    IEnumerator GuardarPartida()
    {
        var gm = GameManager.Instance;
        string fechaHora = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string tiempo    = gm.TiempoPartida.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        string body = $"{{\"idJugador\":{gm.IdJugador}," +
                      $"\"fechaHora\":\"{fechaHora}\"," +
                      $"\"puntaje\":{gm.Puntaje}," +
                      $"\"dificultad\":\"{gm.Dificultad}\"," +
                      $"\"tiempo\":{tiempo}}}";

        using var req = new UnityWebRequest($"{URL_BASE}/partida", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogWarning("No se pudo guardar la partida: " + req.downloadHandler.text);
        else
            Debug.Log("Partida guardada: " + req.downloadHandler.text);
    }

    // ================================================================
    // Verificar logros que se evalúan al terminar la partida:
    // Logro 1 — Hardcore protocol: 20 puertas en dificil
    // Logro 3 — Ambicion creciente: 10000 pts en una partida
    // Logro 4 — Travesia infinita: 10 min en una partida
    // ================================================================
    IEnumerator VerificarLogros()
    {
        var gm = GameManager.Instance;

        // Obtener logros ya desbloqueados en BD para no repetir
        var idsDesbloqueados = new System.Collections.Generic.HashSet<int>();
        using var check = UnityWebRequest.Get($"{URL_BASE}/logros-jugador/{gm.IdJugador}");
        yield return check.SendWebRequest();

        if (check.result == UnityWebRequest.Result.Success)
        {
            var lista = JsonHelper.ParseList<LogroEntry>(check.downloadHandler.text);
            foreach (var l in lista) idsDesbloqueados.Add(l.idLogro);
        }

        // Logro 1 — Hardcore protocol
        if (!idsDesbloqueados.Contains(1) &&
            gm.Dificultad == "dificil" && gm.PuertasVivas >= 20)
            yield return StartCoroutine(DesbloquearLogro(1));

        // Logro 3 — Ambicion creciente
        if (!idsDesbloqueados.Contains(3) && gm.Puntaje >= 10000)
            yield return StartCoroutine(DesbloquearLogro(3));

        // Logro 4 — Travesia infinita
        if (!idsDesbloqueados.Contains(4) && gm.TiempoPartida >= 600f)
            yield return StartCoroutine(DesbloquearLogro(4));
    }

    // ================================================================
    // Guardar logro en BD
    // POST /logro — { idJugador, idLogro, fechaDesbloqueo }
    // ================================================================
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

    [System.Serializable] private class LogroEntry { public int idLogro; }
}
