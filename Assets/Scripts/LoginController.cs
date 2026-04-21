using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Text.RegularExpressions;

// ================================================================
// LoginController
// Cubre CU-01 (Registro) y CU-02 (Inicio de sesión)
//
// VALIDACIONES ACTIVAS (funcionan SIN backend):
//   CU-01 Registro:
//     - Alias: 1-20 chars, solo letras/números/guión bajo
//     - Alias: sin palabras altisonantes (requisito de software)
//     - Correo: formato válido
//     - Año de nacimiento: lógico (entre año actual-100 y año actual-3)
//     - NIP: generado automáticamente por el sistema (no editable)
//   CU-02 Login:
//     - Alias: 1-20 chars, no vacío
//     - NIP: exactamente 4 dígitos numéricos
//     - Campo inválido se marca en ROJO (excepción CU-02)
//
// PARA CONECTAR AL BACKEND:
//   1. Busca "TODO BACKEND" — hay 2 puntos en OnEnable y 2 coroutines al fondo
//   2. Comenta las líneas marcadas con "SIN BACKEND"
//   3. Descomenta las líneas marcadas con "TODO BACKEND"
//   4. Asegúrate que los TextField existan en el UXML con los nombres correctos
// ================================================================

public class LoginController : MonoBehaviour
{
    // TODO BACKEND: cambia esta URL por la del servidor real
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument ui;

    // Paneles
    private VisualElement panelLogin;
    private VisualElement panelRegistro;

    // Navegación entre paneles
    private Button btnIrRegistro;
    private Button btnIrLogin;
    private Button btnVolver;

    // Acciones
    private Button btnLogin;
    private Button btnRegistro;

    // Campos LOGIN
    // UXML: TextField name="CampoAliasLogin"
    // UXML: TextField name="CampoNipLogin"
    private TextField campoAliasLogin;
    private TextField campoNipLogin;

    // Campos REGISTRO
    // UXML: TextField name="CampoAliasRegistro"
    // UXML: TextField name="CampoCorreoRegistro"
    // UXML: TextField name="CampoAnioRegistro"
    private TextField campoAliasRegistro;
    private TextField campoCorreoRegistro;
    private TextField campoAnioRegistro;

    // Labels de feedback
    // UXML: Label name="LabelErrorLogin"
    // UXML: Label name="LabelErrorRegistro"
    // UXML: Label name="LabelNipGenerado"  ← muestra el NIP al jugador
    private Label labelErrorLogin;
    private Label labelErrorRegistro;
    private Label labelNipGenerado;

    // NIP generado internamente — el jugador NO puede escribir el suyo
    private int nipGenerado = -1;

    // Lista de palabras prohibidas en el alias (requisito de software)
    private readonly string[] palabrasProhibidas = {
        "puta", "puto", "mierda", "cabron", "pendejo", "chinga",
        "verga", "culo", "idiota", "estupido", "fuck", "shit", "bitch",
        "damn", "ass", "perra", "culero", "wey", "guey"
    };

    // ----------------------------------------------------------------
    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        // Paneles
        panelLogin    = root.Q<VisualElement>("PanelLogin");
        panelRegistro = root.Q<VisualElement>("PanelRegistro");

        // Navegación
        btnIrRegistro = root.Q<Button>("BtnIrRegistro");
        btnIrLogin    = root.Q<Button>("BtnIrLogin");
        btnVolver     = root.Q<Button>("BtnVolver");

        // Acciones
        btnLogin    = root.Q<Button>("BtnLogin");
        btnRegistro = root.Q<Button>("BtnRegistro");

        // Campos
        campoAliasLogin     = root.Q<TextField>("CampoAliasLogin");
        campoNipLogin       = root.Q<TextField>("CampoNipLogin");
        campoAliasRegistro  = root.Q<TextField>("CampoAliasRegistro");
        campoCorreoRegistro = root.Q<TextField>("CampoCorreoRegistro");
        campoAnioRegistro   = root.Q<TextField>("CampoAnioRegistro");

        // Labels
        labelErrorLogin    = root.Q<Label>("LabelErrorLogin");
        labelErrorRegistro = root.Q<Label>("LabelErrorRegistro");
        labelNipGenerado   = root.Q<Label>("LabelNipGenerado");

        // Estado inicial
        MostrarPanel(panelLogin);
        LimpiarErrores();
        GenerarNuevoNip();

        if (btnIrRegistro != null)
    btnIrRegistro.RegisterCallback<ClickEvent>(e =>
    {
        UISoundManager.Instance.PlayClick();
        OnIrRegistro(e);
    });

    if (btnIrLogin != null)
        btnIrLogin.RegisterCallback<ClickEvent>(e =>
        {
            UISoundManager.Instance.PlayClick();
            OnIrLogin(e);
        });

    if (btnVolver != null)
        btnVolver.RegisterCallback<ClickEvent>(e =>
        {
            UISoundManager.Instance.PlayClick();
            OnVolver(e);
        });

    if (btnRegistro != null)
        btnRegistro.RegisterCallback<ClickEvent>(e =>
        {
            UISoundManager.Instance.PlayClick();
            StartCoroutine(CoroutineRegistro());
        });

    if (btnLogin != null)
        btnLogin.RegisterCallback<ClickEvent>(e =>
        {
            UISoundManager.Instance.PlayClick();
            StartCoroutine(CoroutineLogin());
        });

        // Callbacks navegación
        //if (btnIrRegistro != null) btnIrRegistro.RegisterCallback<ClickEvent>(OnIrRegistro);
        //if (btnIrLogin    != null) btnIrLogin.RegisterCallback<ClickEvent>(OnIrLogin);
        //if (btnVolver     != null) btnVolver.RegisterCallback<ClickEvent>(OnVolver);

        // ── SIN BACKEND: validación local + ir al menú ──────────────
        // if (btnLogin    != null) btnLogin.RegisterCallback<ClickEvent>(OnLoginSinBackend);
        // if (btnRegistro != null) btnRegistro.RegisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineRegistro()));

        // TODO BACKEND: comenta las 2 líneas de arriba y descomenta estas:
        //if (btnLogin    != null) btnLogin.RegisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineLogin()));
        // if (btnRegistro != null) btnRegistro.RegisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineRegistro()));
    }

    void OnDisable()
    {
        //if (btnIrRegistro != null) btnIrRegistro.UnregisterCallback<ClickEvent>(OnIrRegistro);
        //if (btnIrLogin    != null) btnIrLogin.UnregisterCallback<ClickEvent>(OnIrLogin);
        //if (btnVolver     != null) btnVolver.UnregisterCallback<ClickEvent>(OnVolver);
        // if (btnLogin      != null) btnLogin.UnregisterCallback<ClickEvent>(OnLoginSinBackend);
        // if (btnRegistro   != null) btnRegistro.UnregisterCallback<ClickEvent>(OnRegistroSinBackend);


        // if (btnLogin      != null) btnLogin.UnregisterCallback<ClickEvent>(_ => StopCoroutine(CoroutineLogin()));
        // if (btnRegistro   != null) btnRegistro.UnregisterCallback<ClickEvent>(_ => StartCoroutine(CoroutineRegistro()));
    }

    // ================================================================
    // NAVEGACIÓN
    // ================================================================

    void OnIrRegistro(ClickEvent e)
    {
        MostrarPanel(panelRegistro);
        LimpiarErrores();
        GenerarNuevoNip(); // nuevo NIP cada vez que se abre el panel
    }

    void OnIrLogin(ClickEvent e)
    {
        MostrarPanel(panelLogin);
        LimpiarErrores();
    }

    void OnVolver(ClickEvent e) => SceneManager.LoadScene("MenuPrincipal");

    void MostrarPanel(VisualElement panel)
    {
        if (panelLogin    != null) panelLogin.style.display    = DisplayStyle.None;
        if (panelRegistro != null) panelRegistro.style.display = DisplayStyle.None;
        if (panel         != null) panel.style.display         = DisplayStyle.Flex;
    }

    // ================================================================
    // NIP AUTOMÁTICO (CU-01: el sistema genera el NIP, el jugador no lo elige)
    // ================================================================

    void GenerarNuevoNip()
    {
        nipGenerado = Random.Range(1000, 10000); // 4 dígitos: 1000-9999
        if (labelNipGenerado != null)
            labelNipGenerado.text = $"Tu NIP es: {nipGenerado}\n¡Anótalo! Lo necesitarás para iniciar sesión.";
    }

    // ================================================================
    // SIN BACKEND — Flujo temporal mientras no hay API
    // Solo valida localmente y va al menú
    // ================================================================

    // void OnLoginSinBackend(ClickEvent e)
    // {
    //     LimpiarErrores();
    //     string alias = campoAliasLogin?.value?.Trim() ?? "";
    //     string nip   = campoNipLogin?.value?.Trim() ?? "";

    //     if (!ValidarAlias(alias, out string eAlias, esLogin: true))
    //     { MostrarError(labelErrorLogin, eAlias); MarcarRojo(campoAliasLogin); return; }

    //     if (!ValidarNipLogin(nip, out string eNip))
    //     { MostrarError(labelErrorLogin, eNip); MarcarRojo(campoNipLogin); return; }

    //     // Validación local OK — sin backend ir directo al menú
    //     // TODO BACKEND: quita esta línea cuando uses CoroutineLogin()
    //     SceneManager.LoadScene("MenuPrincipal");
    // }

    void OnRegistroSinBackend(ClickEvent e)
    {
        LimpiarErrores();
        string alias  = campoAliasRegistro?.value?.Trim() ?? "";
        string correo = campoCorreoRegistro?.value?.Trim() ?? "";
        string anio   = campoAnioRegistro?.value?.Trim() ?? "";

        if (!ValidarAlias(alias, out string eAlias, esLogin: false))
        { MostrarError(labelErrorRegistro, eAlias); MarcarRojo(campoAliasRegistro); return; }

        if (!ValidarCorreo(correo, out string eCorreo))
        { MostrarError(labelErrorRegistro, eCorreo); MarcarRojo(campoCorreoRegistro); return; }

        if (!ValidarAnio(anio, out string eAnio))
        { MostrarError(labelErrorRegistro, eAnio); MarcarRojo(campoAnioRegistro); return; }

        // Validación local OK — sin backend ir directo al menú
        // TODO BACKEND: quita esta línea cuando uses CoroutineRegistro()
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // VALIDACIONES (activas con y sin backend)
    // ================================================================

    // CU-01 / CU-02 — Alias
    bool ValidarAlias(string alias, out string error, bool esLogin = false)
    {
        if (string.IsNullOrEmpty(alias))
        { error = "El alias no puede estar vacío"; return false; }

        if (alias.Length < 1 || alias.Length > 20)
        { error = "El alias debe tener entre 1 y 20 caracteres"; return false; }

        if (!Regex.IsMatch(alias, @"^[a-zA-Z0-9_áéíóúÁÉÍÓÚñÑ]+$"))
        { error = "El alias contiene caracteres no permitidos"; return false; }

        // Filtro de palabras altisonantes — solo en registro (requisito de software)
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

    // CU-02 — NIP login: exactamente 4 dígitos numéricos
    bool ValidarNipLogin(string nip, out string error)
    {
        if (string.IsNullOrEmpty(nip) || !Regex.IsMatch(nip, @"^\d{4}$"))
        { error = "El NIP debe tener exactamente 4 dígitos numéricos"; return false; }
        error = null;
        return true;
    }

    // Correo: formato básico
    bool ValidarCorreo(string correo, out string error)
    {
        if (string.IsNullOrEmpty(correo))
        { error = "El correo no puede estar vacío"; return false; }
        if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        { error = "Ingresa un correo válido (ej: nombre@correo.com)"; return false; }
        error = null;
        return true;
    }

    // Año lógico: el juego es para niños de 3-13 años
    // Rango permitido: año actual - 100 hasta año actual - 3
    bool ValidarAnio(string anioStr, out string error)
    {
        if (string.IsNullOrEmpty(anioStr))
        { error = "El año de nacimiento no puede estar vacío"; return false; }

        if (!int.TryParse(anioStr, out int anio))
        { error = "El año debe ser un número"; return false; }

        int anioActual = System.DateTime.Now.Year;
        int minimo     = anioActual - 100;  // máximo 100 años de edad
        int maximo     = anioActual - 3;    // mínimo 3 años de edad

        if (anio < minimo || anio > maximo)
        { error = $"Año inválido. Debe estar entre {minimo} y {maximo}"; return false; }

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
    // TODO BACKEND — INICIO DE SESIÓN (CU-02)
    //
    // Endpoint esperado: POST /login
    // Body:    { "alias": "Juan", "nip": 1234 }
    // OK:      { "idJugador": 1, "alias": "Juan" }
    // Error:   { "error": "Cuenta no encontrada" }
    // ================================================================

    IEnumerator CoroutineLogin()
    {
        LimpiarErrores();
        string alias = campoAliasLogin?.value?.Trim() ?? "";
        string nip   = campoNipLogin?.value?.Trim() ?? "";
    
        // Validaciones locales primero
        if (!ValidarAlias(alias, out string eAlias, esLogin: true))
        { MostrarError(labelErrorLogin, eAlias); MarcarRojo(campoAliasLogin); yield break; }
        if (!ValidarNipLogin(nip, out string eNip))
        { MostrarError(labelErrorLogin, eNip); MarcarRojo(campoNipLogin); yield break; }
    
        // Llamada al servidor
        string body = $"{{\"alias\":\"{alias}\",\"nip\":{nip}}}";
        using var req = new UnityWebRequest($"{URL_BASE}/login", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
    
        if (req.result != UnityWebRequest.Result.Success)
        { MostrarError(labelErrorLogin, "Error de conexión. Intenta de nuevo."); yield break; }
    
        var resp = JsonUtility.FromJson<RespuestaLogin>(req.downloadHandler.text);
        if (!string.IsNullOrEmpty(resp.error))
        { MostrarError(labelErrorLogin, resp.error); yield break; }
    
        // Guardar sesión en GameManager
        GameManager.Instance.IdJugador    = resp.idJugador;
        GameManager.Instance.AliasJugador = resp.alias;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // TODO BACKEND — REGISTRO (CU-01)
    //
    // Endpoint esperado: POST /registro
    // Body:    { "alias": "Juan", "correo": "j@x.com", "anioNacimiento": 2013, "nip": 4827 }
    // OK:      { "idJugador": 5, "alias": "Juan" }
    // Error:   { "error": "El alias ya existe" }
    //
    // Nota: el NIP ya fue generado por el sistema (nipGenerado)
    //       el jugador solo lo VE, no lo escribe
    // ================================================================

    IEnumerator CoroutineRegistro()
    {
        LimpiarErrores();
        string alias  = campoAliasRegistro?.value?.Trim() ?? "";
        string correo = campoCorreoRegistro?.value?.Trim() ?? "";
        string anio   = campoAnioRegistro?.value?.Trim() ?? "";
    
        // Validaciones locales primero
        if (!ValidarAlias(alias, out string eAlias, esLogin: false))
        { MostrarError(labelErrorRegistro, eAlias); MarcarRojo(campoAliasRegistro); yield break; }
        if (!ValidarCorreo(correo, out string eCorreo))
        { MostrarError(labelErrorRegistro, eCorreo); MarcarRojo(campoCorreoRegistro); yield break; }
        if (!ValidarAnio(anio, out string eAnio))
        { MostrarError(labelErrorRegistro, eAnio); MarcarRojo(campoAnioRegistro); yield break; }
    
        // Llamada al servidor — se envía el NIP generado por el sistema
        string body = $"{{\"alias\":\"{alias}\",\"correo\":\"{correo}\"," +
                      $"\"anioNacimiento\":{anio},\"nip\":{nipGenerado}}}";
        using var req = new UnityWebRequest($"{URL_BASE}/registro", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
    
        if (req.result != UnityWebRequest.Result.Success)
        { MostrarError(labelErrorRegistro, "Error de conexión. Intenta de nuevo."); yield break; }
    
        var resp = JsonUtility.FromJson<RespuestaRegistro>(req.downloadHandler.text);
        if (!string.IsNullOrEmpty(resp.error))
        { MostrarError(labelErrorRegistro, resp.error); yield break; }
    
        // CU-01 paso 6: sesión automática tras registro exitoso
        GameManager.Instance.IdJugador    = resp.idJugador;
        GameManager.Instance.AliasJugador = resp.alias;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // Clases de datos — descomentar junto con las coroutines
    // ================================================================
    [System.Serializable] private class RespuestaLogin   { public int idJugador; public string alias; public string error; }
    [System.Serializable] private class RespuestaRegistro{ public int idJugador; public string alias; public string error; }
}