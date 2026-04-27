using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

public class LoginController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument ui;

    private VisualElement panelLogin;
    private VisualElement panelRegistro;

    private Button btnIrRegistro;
    private Button btnIrLogin;
    private Button btnVolver;
    private Button btnLogin;
    private Button btnRegistro;

    private TextField campoAliasLogin;
    private TextField campoNipLogin;
    private TextField campoAliasRegistro;
    private TextField campoCorreoRegistro;
    private TextField campoAnioRegistro;

    private Label labelErrorLogin;
    private Label labelErrorRegistro;
    private Label labelNipGenerado;

    private int nipGenerado = -1;

    private readonly string[] palabrasProhibidas = {
        "puta", "puto", "mierda", "cabron", "pendejo", "chinga",
        "verga", "culo", "idiota", "estupido", "fuck", "shit", "bitch",
        "damn", "ass", "perra", "culero", "wey", "guey"
    };

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        panelLogin    = root.Q<VisualElement>("PanelLogin");
        panelRegistro = root.Q<VisualElement>("PanelRegistro");

        btnIrRegistro = root.Q<Button>("BtnIrRegistro");
        btnIrLogin    = root.Q<Button>("BtnIrLogin");
        btnVolver     = root.Q<Button>("BtnVolver");
        btnLogin      = root.Q<Button>("BtnLogin");
        btnRegistro   = root.Q<Button>("BtnRegistro");

        campoAliasLogin     = root.Q<TextField>("CampoAliasLogin");
        campoNipLogin       = root.Q<TextField>("CampoNipLogin");
        campoAliasRegistro  = root.Q<TextField>("CampoAliasRegistro");
        campoCorreoRegistro = root.Q<TextField>("CampoCorreoRegistro");
        campoAnioRegistro   = root.Q<TextField>("CampoAnioRegistro");

        labelErrorLogin    = root.Q<Label>("LabelErrorLogin");
        labelErrorRegistro = root.Q<Label>("LabelErrorRegistro");
        labelNipGenerado   = root.Q<Label>("LabelNipGenerado");

        MostrarPanel(panelLogin);
        LimpiarErrores();
        GenerarNuevoNip();

        if (btnIrRegistro != null) btnIrRegistro.RegisterCallback<ClickEvent>(OnIrRegistro);
        if (btnIrLogin    != null) btnIrLogin.RegisterCallback<ClickEvent>(OnIrLogin);
        if (btnVolver     != null) btnVolver.RegisterCallback<ClickEvent>(OnVolver);
        if (btnLogin      != null) btnLogin.RegisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineLogin()));
        if (btnRegistro   != null) btnRegistro.RegisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineRegistro()));
    }

    void OnDisable()
    {
        if (btnIrRegistro != null) btnIrRegistro.UnregisterCallback<ClickEvent>(OnIrRegistro);
        if (btnIrLogin    != null) btnIrLogin.UnregisterCallback<ClickEvent>(OnIrLogin);
        if (btnVolver     != null) btnVolver.UnregisterCallback<ClickEvent>(OnVolver);
    }

    void OnIrRegistro(ClickEvent e) { MostrarPanel(panelRegistro); LimpiarErrores(); GenerarNuevoNip(); }
    void OnIrLogin(ClickEvent e)    { MostrarPanel(panelLogin);    LimpiarErrores(); }
    void OnVolver(ClickEvent e)     => SceneManager.LoadScene("MenuPrincipal");

    void MostrarPanel(VisualElement panel)
    {
        if (panelLogin    != null) panelLogin.style.display    = DisplayStyle.None;
        if (panelRegistro != null) panelRegistro.style.display = DisplayStyle.None;
        if (panel         != null) panel.style.display         = DisplayStyle.Flex;
    }

    void GenerarNuevoNip()
    {
        nipGenerado = Random.Range(1000, 10000);
        if (labelNipGenerado != null)
            labelNipGenerado.text = $"{nipGenerado}";
    }

    // ================================================================
    // LOGIN
    // POST /login — { alias, nip }
    // OK:    { idJugador, alias }
    // Error: { error: "Cuenta no encontrada" }
    // ================================================================
    IEnumerator CoroutineLogin()
    {
        LimpiarErrores();
        string alias = campoAliasLogin?.value?.Trim() ?? "";
        string nip   = campoNipLogin?.value?.Trim() ?? "";

        if (!ValidarAlias(alias, out string eAlias, esLogin: true))
        { MostrarError(labelErrorLogin, eAlias); MarcarRojo(campoAliasLogin); yield break; }
        if (!ValidarNipLogin(nip, out string eNip))
        { MostrarError(labelErrorLogin, eNip); MarcarRojo(campoNipLogin); yield break; }

        string body = $"{{\"alias\":\"{alias}\",\"nip\":{nip}}}";
        using var req = new UnityWebRequest($"{URL_BASE}/login", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            try
            {
                var err = JsonUtility.FromJson<RespuestaError>(req.downloadHandler.text);
                MostrarError(labelErrorLogin, !string.IsNullOrEmpty(err.error)
                    ? err.error
                    : "Cuenta no encontrada");
            }
            catch { MostrarError(labelErrorLogin, "Cuenta no encontrada"); }
            yield break;
        }

        var resp = JsonUtility.FromJson<RespuestaLogin>(req.downloadHandler.text);
        if (resp == null || resp.idJugador <= 0)
        { MostrarError(labelErrorLogin, "Cuenta no encontrada"); yield break; }

        GameManager.Instance.IdJugador    = resp.idJugador;
        GameManager.Instance.AliasJugador = alias;
        GameManager.Instance.GuardarSesion();

        yield return StartCoroutine(VerificarPrimeraPartida(resp.idJugador));

        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // REGISTRO
    // POST /registro — { alias, correo, anioNacimiento, nip }
    // OK:    { idJugador, alias }
    // Error: { error: "..." }
    // ================================================================
    IEnumerator CoroutineRegistro()
    {
        LimpiarErrores();
        string alias  = campoAliasRegistro?.value?.Trim() ?? "";
        string correo = campoCorreoRegistro?.value?.Trim() ?? "";
        string anio   = campoAnioRegistro?.value?.Trim() ?? "";

        if (!ValidarAlias(alias, out string eAlias, esLogin: false))
        { MostrarError(labelErrorRegistro, eAlias); MarcarRojo(campoAliasRegistro); yield break; }
        if (!ValidarCorreo(correo, out string eCorreo))
        { MostrarError(labelErrorRegistro, eCorreo); MarcarRojo(campoCorreoRegistro); yield break; }
        if (!ValidarAnio(anio, out string eAnio))
        { MostrarError(labelErrorRegistro, eAnio); MarcarRojo(campoAnioRegistro); yield break; }

        string body = $"{{\"alias\":\"{alias}\",\"correo\":\"{correo}\"," +
                      $"\"anioNacimiento\":{anio},\"nip\":{nipGenerado}}}";
        using var req = new UnityWebRequest($"{URL_BASE}/registro", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            try
            {
                var err = JsonUtility.FromJson<RespuestaError>(req.downloadHandler.text);
                MostrarError(labelErrorRegistro, !string.IsNullOrEmpty(err.error)
                    ? err.error
                    : "Error al registrar. Intenta con otro alias.");
            }
            catch { MostrarError(labelErrorRegistro, "Error de conexión. Intenta de nuevo."); }
            yield break;
        }

        var resp = JsonUtility.FromJson<RespuestaLogin>(req.downloadHandler.text);
        if (resp == null || resp.idJugador <= 0)
        { MostrarError(labelErrorRegistro, "Error al registrar. Intenta con otro alias."); yield break; }

        GameManager.Instance.IdJugador    = resp.idJugador;
        GameManager.Instance.AliasJugador = alias;
        GameManager.Instance.EsPrimeraPartida = true;
        GameManager.Instance.SkinSeleccionada = 0;
        GameManager.Instance.GuardarSesion();

        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // Verificar primera partida desde BD
    // ================================================================
    IEnumerator VerificarPrimeraPartida(int idJugador)
    {
        using var req = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<RespuestaPuntaje>(req.downloadHandler.text);
            GameManager.Instance.EsPrimeraPartida = (resp != null && resp.puntajeTotal == 0);
        }
        else
            GameManager.Instance.EsPrimeraPartida = false;

        // Resetear skin al cambiar de sesión
        GameManager.Instance.SkinSeleccionada = 0;
    }

    // ================================================================
    // VALIDACIONES
    // ================================================================
    bool ValidarAlias(string alias, out string error, bool esLogin = false)
    {
        if (string.IsNullOrEmpty(alias))
        { error = "El alias no puede estar vacío"; return false; }
        if (alias.Length < 1 || alias.Length > 20)
        { error = "El alias debe tener entre 1 y 20 caracteres"; return false; }
        if (!Regex.IsMatch(alias, @"^[a-zA-Z0-9_áéíóúÁÉÍÓÚñÑ]+$"))
        { error = "El alias contiene caracteres no permitidos"; return false; }
        if (!esLogin)
        {
            string aliasLower = alias.ToLower();
            foreach (var palabra in palabrasProhibidas)
                if (aliasLower.Contains(palabra))
                { error = "El alias contiene palabras no permitidas"; return false; }
        }
        error = null;
        return true;
    }

    bool ValidarNipLogin(string nip, out string error)
    {
        if (string.IsNullOrEmpty(nip) || !Regex.IsMatch(nip, @"^\d{4}$"))
        { error = "El NIP debe tener exactamente 4 dígitos numéricos"; return false; }
        error = null;
        return true;
    }

    bool ValidarCorreo(string correo, out string error)
    {
        if (string.IsNullOrEmpty(correo))
        { error = "El correo no puede estar vacío"; return false; }
        if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        { error = "Ingresa un correo válido (ej: nombre@correo.com)"; return false; }
        error = null;
        return true;
    }

    bool ValidarAnio(string anioStr, out string error)
    {
        if (string.IsNullOrEmpty(anioStr))
        { error = "El año de nacimiento no puede estar vacío"; return false; }
        if (!int.TryParse(anioStr, out int anio))
        { error = "El año debe ser un número"; return false; }
        int anioActual = System.DateTime.Now.Year;
        if (anio < anioActual - 100 || anio > anioActual - 3)
        { error = $"Año inválido. Debe estar entre {anioActual - 100} y {anioActual - 3}"; return false; }
        error = null;
        return true;
    }

    // ================================================================
    // HELPERS UI
    // ================================================================
    void MostrarError(Label label, string mensaje)
    {
        if (label == null) return;
        label.text = mensaje;
        label.style.color   = new StyleColor(new Color(1f, 0.3f, 0.3f));
        label.style.display = DisplayStyle.Flex;
    }

    void MarcarRojo(VisualElement campo)
    {
        if (campo == null) return;
        campo.style.borderBottomColor = new StyleColor(Color.red);
        campo.style.borderBottomWidth = 2;
    }

    void LimpiarErrores()
    {
        if (labelErrorLogin    != null) { labelErrorLogin.text = "";    labelErrorLogin.style.display    = DisplayStyle.None; }
        if (labelErrorRegistro != null) { labelErrorRegistro.text = ""; labelErrorRegistro.style.display = DisplayStyle.None; }
        foreach (var campo in new VisualElement[] {
            campoAliasLogin, campoNipLogin,
            campoAliasRegistro, campoCorreoRegistro, campoAnioRegistro })
        {
            if (campo == null) continue;
            campo.style.borderBottomColor = StyleKeyword.Null;
            campo.style.borderBottomWidth = StyleKeyword.Null;
        }
    }

    // ================================================================
    // Clases de datos
    // ================================================================
    [System.Serializable] private class RespuestaLogin   { public int idJugador; public string alias; }
    [System.Serializable] private class RespuestaError   { public string error; public string name; public string message; }
    [System.Serializable] private class RespuestaPuntaje { public int puntajeTotal; }
}