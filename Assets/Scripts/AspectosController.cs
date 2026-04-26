using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class AspectosController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument ui;

    private Button btnSeleccionar;
    private Button btnSeleccionado;
    private Button btnVolver;
    private VisualElement imagenPreview;

    private Button[] btnSkins = new Button[12];

    private readonly int[] puntajesDesbloqueo = {
        0,      // Skin 1  — gratis
        5000,   // Skin 2
        10000,  // Skin 3
        15000,  // Skin 4
        20000,  // Skin 5
        25000,  // Skin 6
        30000,  // Skin 7
        35000,  // Skin 8
        40000,  // Skin 9
        45000,  // Skin 10
        50000,  // Skin 11
        100000  // Skin 12
    };

    private int skinSeleccionadaActual = 0;
    private int skinEnPreview          = 0;
    private HashSet<int> aspectosDesbloqueados = new HashSet<int>();

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnSeleccionar  = root.Q<Button>("BtnSeleccionar");
        btnSeleccionado = root.Q<Button>("BtnSeleccionado");
        btnVolver       = root.Q<Button>("BtnVolver");
        imagenPreview   = root.Q<VisualElement>("ImagenPreview");

        for (int i = 0; i < 12; i++)
            btnSkins[i] = root.Q<Button>($"BtnSkin{i + 1}");

        // Empezar todo bloqueado mientras carga
        AplicarEstadosSkins(new HashSet<int> { 1 });

        skinSeleccionadaActual = GameManager.Instance?.SkinSeleccionada ?? 0;
        skinEnPreview          = skinSeleccionadaActual;

        for (int i = 0; i < 12; i++)
        {
            int idx = i;
            if (btnSkins[idx] != null)
                btnSkins[idx].RegisterCallback<ClickEvent>(e =>
                {
                    UISoundManager.Instance.PlayClick();
                    OnClickSkin(idx);
                });
        }

        if (btnSeleccionar != null)
            btnSeleccionar.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnSeleccionar(e);
            });

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                OnVolver(e);
            });

        StartCoroutine(CargarAspectosDesdeDB());
    }

    void OnDisable() { }

    // ================================================================
    // 1. Obtener puntaje total del jugador
    // 2. Obtener aspectos ya registrados en BD
    // 3. Guardar en BD los que apliquen y no estén aún
    // 4. Consultar estado final y aplicar UI
    // ================================================================
    IEnumerator CargarAspectosDesdeDB()
    {
        int idJugador = GameManager.Instance.IdJugador;

        // Paso 1 — puntaje total acumulado
        int puntajeTotal = 0;
        using var reqPuntaje = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return reqPuntaje.SendWebRequest();

        if (reqPuntaje.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<RespuestaPuntaje>(reqPuntaje.downloadHandler.text);
            if (resp != null) puntajeTotal = resp.puntajeTotal;
        }
        else
            Debug.LogWarning("No se pudo obtener puntaje total: " + reqPuntaje.downloadHandler.text);

        // Paso 2 — aspectos ya en BD
        var idsEnBD = new HashSet<int>();
        using var reqAspectos = UnityWebRequest.Get($"{URL_BASE}/aspectos-jugador/{idJugador}");
        yield return reqAspectos.SendWebRequest();

        if (reqAspectos.result == UnityWebRequest.Result.Success)
        {
            var lista = JsonHelper.ParseList<AspectoEntry>(reqAspectos.downloadHandler.text);
            foreach (var a in lista) idsEnBD.Add(a.idAspecto);
        }
        else
            Debug.LogWarning("No se pudo obtener aspectos: " + reqAspectos.downloadHandler.text);

        // Paso 3 — guardar en BD los que apliquen y no estén registrados
        for (int i = 0; i < 12; i++)
        {
            int idAspecto = i + 1;
            if (puntajeTotal >= puntajesDesbloqueo[i] && !idsEnBD.Contains(idAspecto))
                yield return StartCoroutine(GuardarAspectoEnBD(idAspecto));
        }

        // Paso 4 — consultar estado final
        using var reqFinal = UnityWebRequest.Get($"{URL_BASE}/aspectos-jugador/{idJugador}");
        yield return reqFinal.SendWebRequest();

        aspectosDesbloqueados = new HashSet<int> { 1 }; // skin 1 siempre gratis
        if (reqFinal.result == UnityWebRequest.Result.Success)
        {
            var listaFinal = JsonHelper.ParseList<AspectoEntry>(reqFinal.downloadHandler.text);
            foreach (var a in listaFinal) aspectosDesbloqueados.Add(a.idAspecto);
        }

        AplicarEstadosSkins(aspectosDesbloqueados);
        ActualizarPreview(skinSeleccionadaActual);
    }

    // ================================================================
    // POST /aspecto — { idJugador, idAspecto, fechaDesbloqueo }
    // ================================================================
    IEnumerator GuardarAspectoEnBD(int idAspecto)
    {
        string fecha = System.DateTime.Now.ToString("yyyy-MM-dd");
        string body  = $"{{\"idJugador\":{GameManager.Instance.IdJugador}," +
                       $"\"idAspecto\":{idAspecto}," +
                       $"\"fechaDesbloqueo\":\"{fecha}\"}}";

        using var req = new UnityWebRequest($"{URL_BASE}/aspecto", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogWarning($"No se pudo guardar aspecto {idAspecto}: " + req.downloadHandler.text);
        else
            Debug.Log($"Aspecto {idAspecto} desbloqueado en BD.");
    }

    // ================================================================
    // UI
    // ================================================================
    void AplicarEstadosSkins(HashSet<int> desbloqueados)
    {
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;
            bool desbloqueada = desbloqueados.Contains(i + 1);
            btnSkins[i].style.opacity = desbloqueada ? 1f : 0.35f;
            btnSkins[i].SetEnabled(desbloqueada);
            btnSkins[i].tooltip = desbloqueada
                ? ""
                : $"Necesitas {puntajesDesbloqueo[i]} puntos para desbloquear";
        }
    }

    void OnClickSkin(int idx)
    {
        skinEnPreview = idx;
        ActualizarPreview(idx);
    }

    void ActualizarPreview(int idx)
    {
        for (int i = 0; i < 12; i++)
        {
            if (btnSkins[i] == null) continue;
            bool selected = (i == idx);
            btnSkins[i].style.borderTopWidth    =
            btnSkins[i].style.borderBottomWidth =
            btnSkins[i].style.borderLeftWidth   =
            btnSkins[i].style.borderRightWidth  = selected ? 3 : 0;

            var color = new StyleColor(Color.cyan);
            btnSkins[i].style.borderTopColor    =
            btnSkins[i].style.borderBottomColor =
            btnSkins[i].style.borderLeftColor   =
            btnSkins[i].style.borderRightColor  = color;
        }

        bool yaEsLaActual = (idx == skinSeleccionadaActual);
        if (btnSeleccionar  != null) btnSeleccionar.style.display  = yaEsLaActual ? DisplayStyle.None : DisplayStyle.Flex;
        if (btnSeleccionado != null) btnSeleccionado.style.display = yaEsLaActual ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void OnSeleccionar(ClickEvent e)
    {
        skinSeleccionadaActual = skinEnPreview;

        if (GameManager.Instance != null)
            GameManager.Instance.SkinSeleccionada = skinSeleccionadaActual;

        ActualizarPreview(skinSeleccionadaActual);
    }

    void OnVolver(ClickEvent e) => SceneManager.LoadScene("MenuPrincipal");

    [System.Serializable] private class AspectoEntry     { public int idAspecto; }
    [System.Serializable] private class RespuestaPuntaje { public int puntajeTotal; }
}