using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// IDs de logros en BD:
// 1 = Hardcore protocol   → se desbloquea en GameOverController
// 2 = Identidad completa  → se verifica aquí (aspectos desbloqueados)
// 3 = Ambicion creciente  → se desbloquea en GameOverController
// 4 = Travesia infinita   → se desbloquea en GameOverController
// 5 = Dominio absoluto    → se desbloquea en MenuController
// 6 = Overflow            → se verifica aquí (puntaje total acumulado)

public class LogrosController : MonoBehaviour
{
    private const string URL_BASE      = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";
    private const int    TOTAL_ASPECTOS = 12;
    private const int    PUNTAJE_OVERFLOW = 1000000;

    private UIDocument ui;
    private Button        btnVolver;
    private VisualElement contenedorLogros;
    private Label         labelSinDatos;
    private VisualElement[] logros = new VisualElement[6];

    // Mapeo índice UI (0-5) → idLogro en BD (1-6)
    private readonly int[] idLogroPorIndice = { 1, 2, 3, 4, 5, 6 };

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnVolver        = root.Q<Button>("BtnVolver");
        contenedorLogros = root.Q<VisualElement>("ContenedorLogros");
        labelSinDatos    = root.Q<Label>("LabelSinDatos");

        for (int i = 0; i < 6; i++)
            logros[i] = root.Q<VisualElement>($"Logro{i + 1}");

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                VolverMenu(e);
            });

        // Empezar todos bloqueados mientras carga
        AplicarEstadosLogros(new bool[] { false, false, false, false, false, false });

        if (GameManager.Instance != null && GameManager.Instance.HaySesion)
            StartCoroutine(CargarYVerificarLogros());
        else
            MostrarSinSesion();
    }

    void OnDisable() { }

    // ================================================================
    // Flujo principal
    // ================================================================
    IEnumerator CargarYVerificarLogros()
    {
        int idJugador = GameManager.Instance.IdJugador;

        // Paso 1 — obtener logros ya desbloqueados en BD
        var idsDesbloqueados = new HashSet<int>();
        using var req1 = UnityWebRequest.Get($"{URL_BASE}/logros-jugador/{idJugador}");
        yield return req1.SendWebRequest();

        if (req1.result == UnityWebRequest.Result.Success)
        {
            var lista = JsonHelper.ParseList<LogroEntry>(req1.downloadHandler.text);
            foreach (var l in lista) idsDesbloqueados.Add(l.idLogro);
        }

        // Paso 2 — verificar logros pendientes que se evalúan aquí
        // Logro 2 — Identidad completa
        if (!idsDesbloqueados.Contains(2))
            yield return StartCoroutine(VerificarIdentidadCompleta(idsDesbloqueados));

        // Logro 6 — Overflow
        if (!idsDesbloqueados.Contains(6))
            yield return StartCoroutine(VerificarOverflow(idsDesbloqueados));

        // Paso 3 — consultar estado final actualizado en BD
        var idsFinales = new HashSet<int>();
        using var req2 = UnityWebRequest.Get($"{URL_BASE}/logros-jugador/{idJugador}");
        yield return req2.SendWebRequest();

        if (req2.result == UnityWebRequest.Result.Success)
        {
            var lista2 = JsonHelper.ParseList<LogroEntry>(req2.downloadHandler.text);
            foreach (var l in lista2) idsFinales.Add(l.idLogro);
        }

        // Paso 4 — mostrar UI
        using var reqPuntaje = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return reqPuntaje.SendWebRequest();

        bool haJugado = false;
        if (reqPuntaje.result == UnityWebRequest.Result.Success)
        {
            var respPuntaje = JsonUtility.FromJson<RespuestaPuntaje>(reqPuntaje.downloadHandler.text);
            haJugado = respPuntaje != null && respPuntaje.puntajeTotal > 0;
        }

        if (!haJugado)
            MostrarSinPartidas();
        else
        {
            MostrarLogros();
            bool[] estados = new bool[6];
            for (int i = 0; i < 6; i++)
                estados[i] = idsFinales.Contains(idLogroPorIndice[i]);
            AplicarEstadosLogros(estados);
        }
    }

    // ================================================================
    // Logro 2 — Identidad completa: todos los aspectos desbloqueados
    // GET /aspectos-jugador/:idJugador → cuenta si tiene los 12
    // ================================================================
    IEnumerator VerificarIdentidadCompleta(HashSet<int> idsYaDesbloqueados)
    {
        int idJugador = GameManager.Instance.IdJugador;
        using var req = UnityWebRequest.Get($"{URL_BASE}/aspectos-jugador/{idJugador}");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        var aspectos = JsonHelper.ParseList<AspectoEntry>(req.downloadHandler.text);
        if (aspectos != null && aspectos.Count >= TOTAL_ASPECTOS)
            yield return StartCoroutine(DesbloquearLogro(2));
    }

    // ================================================================
    // Logro 6 — Overflow: 1,000,000 pts acumulados
    // GET /puntaje-total/:idJugador → { puntajeTotal }
    // ================================================================
    IEnumerator VerificarOverflow(HashSet<int> idsYaDesbloqueados)
    {
        int idJugador = GameManager.Instance.IdJugador;
        using var req = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        var resp = JsonUtility.FromJson<RespuestaPuntaje>(req.downloadHandler.text);
        if (resp != null && resp.puntajeTotal >= PUNTAJE_OVERFLOW)
            yield return StartCoroutine(DesbloquearLogro(6));
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

    // ================================================================
    // UI
    // ================================================================
    void AplicarEstadosLogros(bool[] desbloqueados)
    {
        for (int i = 0; i < 6; i++)
        {
            if (logros[i] == null) continue;
            logros[i].style.opacity = desbloqueados[i] ? 1f : 0.35f;
        }
    }

    void MostrarSinPartidas()
    {
        if (contenedorLogros != null) contenedorLogros.style.display = DisplayStyle.None;
        if (labelSinDatos    != null)
        {
            labelSinDatos.text          = "Aún no has jugado ninguna partida.\n¡Juega para desbloquear logros!";
            labelSinDatos.style.display = DisplayStyle.Flex;
        }
    }

    void MostrarLogros()
    {
        if (contenedorLogros != null) contenedorLogros.style.display = DisplayStyle.Flex;
        if (labelSinDatos    != null) labelSinDatos.style.display    = DisplayStyle.None;
    }

    void MostrarSinSesion()
    {
        if (contenedorLogros != null) contenedorLogros.style.display = DisplayStyle.None;
        if (labelSinDatos    != null)
        {
            labelSinDatos.text          = "Inicia sesión para ver tus logros";
            labelSinDatos.style.display = DisplayStyle.Flex;
        }
    }

    void VolverMenu(ClickEvent evt) => SceneManager.LoadScene("MenuPrincipal");

    [System.Serializable] private class LogroEntry      { public int idLogro; }
    [System.Serializable] private class AspectoEntry    { public int idAspecto; }
    [System.Serializable] private class RespuestaPuntaje { public int puntajeTotal; }
}
