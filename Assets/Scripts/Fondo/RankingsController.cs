using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class RankingsController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";
    private const int    TIMEOUT  = 5; // segundos antes de mostrar error

    private UIDocument ui;

    private VisualElement panelSemanal;
    private VisualElement panelHistorico;

    private Button btnVerHistorico;
    private Button btnVerSemanal;
    private Button btnVolverHistorico;
    private Button btnVolverSemanal;

    private VisualElement listaHistorico;
    private VisualElement listaSemanal;

    private Label labelCargandoHistorico;
    private Label labelCargandoSemanal;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        panelSemanal   = root.Q<VisualElement>("PanelSemanal");
        panelHistorico = root.Q<VisualElement>("PanelHistorico");

        btnVerHistorico    = root.Q<Button>("BtnVerHistorico");
        btnVerSemanal      = root.Q<Button>("BtnVerSemanal");
        btnVolverHistorico = root.Q<Button>("BtnVolverHistorico");
        btnVolverSemanal   = root.Q<Button>("BtnVolverSemanal");

        listaHistorico = root.Q<VisualElement>("ListaHistorico");
        listaSemanal   = root.Q<VisualElement>("ListaSemanal");

        labelCargandoHistorico = root.Q<Label>("LabelCargandoHistorico");
        labelCargandoSemanal   = root.Q<Label>("LabelCargandoSemanal");

        if (panelSemanal   != null) panelSemanal.style.display   = DisplayStyle.Flex;
        if (panelHistorico != null) panelHistorico.style.display = DisplayStyle.None;

        if (btnVerHistorico    != null) btnVerHistorico.RegisterCallback<ClickEvent>(MostrarHistorico);
        if (btnVerSemanal      != null) btnVerSemanal.RegisterCallback<ClickEvent>(MostrarSemanal);
        if (btnVolverHistorico != null) btnVolverHistorico.RegisterCallback<ClickEvent>(VolverMenu);
        if (btnVolverSemanal   != null) btnVolverSemanal.RegisterCallback<ClickEvent>(VolverMenu);

        StartCoroutine(CargarRanking("semanal"));
        StartCoroutine(CargarRanking("historico"));
    }

    void OnDisable()
    {
        if (btnVerHistorico    != null) btnVerHistorico.UnregisterCallback<ClickEvent>(MostrarHistorico);
        if (btnVerSemanal      != null) btnVerSemanal.UnregisterCallback<ClickEvent>(MostrarSemanal);
        if (btnVolverHistorico != null) btnVolverHistorico.UnregisterCallback<ClickEvent>(VolverMenu);
        if (btnVolverSemanal   != null) btnVolverSemanal.UnregisterCallback<ClickEvent>(VolverMenu);
    }

    IEnumerator CargarRanking(string tipo)
    {
        string url = tipo == "semanal"
                   ? $"{URL_BASE}/ranking-semanal"
                   : $"{URL_BASE}/ranking-historico";

        Label         labelCargando = tipo == "semanal" ? labelCargandoSemanal  : labelCargandoHistorico;
        VisualElement lista         = tipo == "semanal" ? listaSemanal          : listaHistorico;

        MostrarMensajeLista(labelCargando, lista, "Cargando...");

        UnityWebRequest req = null;
        try
        {
            req = UnityWebRequest.Get(url);
            req.timeout = TIMEOUT;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Rankings] Error creando request: {ex.Message}");
            MostrarMensajeLista(labelCargando, lista, "Servidor no disponible");
            yield break;
        }

        using (req)
        {
            yield return req.SendWebRequest();

            // Cualquier tipo de fallo de red
            if (req.result == UnityWebRequest.Result.ConnectionError    ||
                req.result == UnityWebRequest.Result.ProtocolError      ||
                req.result == UnityWebRequest.Result.DataProcessingError||
                req.result != UnityWebRequest.Result.Success)
            {
                MostrarMensajeLista(labelCargando, lista, "Servidor no disponible.\nIntenta mas tarde.");
                Debug.LogWarning($"[Rankings] {tipo}: {req.error}");
                yield break;
            }

            // Respuesta vacía
            string json = req.downloadHandler?.text;
            if (string.IsNullOrEmpty(json))
            {
                MostrarMensajeLista(labelCargando, lista, "Sin datos disponibles");
                yield break;
            }

            // Intentar parsear — si falla mostrar error en lugar de crash
            List<RankingEntry> entradas = null;
            try
            {
                entradas = JsonHelper.ParseList<RankingEntry>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Rankings] Error parseando JSON: {ex.Message}");
                MostrarMensajeLista(labelCargando, lista, "Error al leer datos");
                yield break;
            }

            // Todo OK — ocultar "Cargando..." y mostrar lista
            if (labelCargando != null) labelCargando.style.display = DisplayStyle.None;
            PoblarLista(lista, entradas);
        }
    }

    // Muestra un mensaje en el label Y en la lista (por si el label no existe)
    void MostrarMensajeLista(Label label, VisualElement lista, string mensaje)
    {
        if (label != null)
        {
            label.text = mensaje;
            label.style.display = DisplayStyle.Flex;
        }
        else if (lista != null)
        {
            lista.Clear();
            var lbl = new Label(mensaje);
            lbl.style.color       = new StyleColor(Color.white);
            lbl.style.fontSize    = 20;
            lbl.style.whiteSpace  = WhiteSpace.Normal;
            lista.Add(lbl);
        }
    }

    void PoblarLista(VisualElement lista, List<RankingEntry> entradas)
    {
        if (lista == null) return;
        lista.Clear();

        if (entradas == null || entradas.Count == 0)
        {
            var vacio = new Label("Sin datos disponibles");
            vacio.style.color    = new StyleColor(Color.white);
            vacio.style.fontSize = 24;
            lista.Add(vacio);
            return;
        }

        foreach (var entrada in entradas)
        {
            var fila = new VisualElement();
            fila.style.flexDirection     = FlexDirection.Row;
            fila.style.justifyContent    = Justify.SpaceBetween;
            fila.style.paddingTop        = 8;
            fila.style.paddingBottom     = 8;
            fila.style.paddingLeft       = 10;
            fila.style.paddingRight      = 10;
            fila.style.borderBottomWidth = 1;
            fila.style.borderBottomColor = new StyleColor(new Color(1, 1, 1, 0.3f));

            var lPos   = new Label($"{entrada.posicion}.");
            var lAlias = new Label(entrada.alias);
            var lPts   = new Label($"{entrada.puntaje} pts");

            foreach (var l in new[] { lPos, lAlias, lPts })
            {
                l.style.color                   = new StyleColor(Color.white);
                l.style.fontSize                = 22;
                l.style.unityFontStyleAndWeight = FontStyle.Bold;
            }

            lPos.style.minWidth         = 40;
            lPts.style.unityTextAlign   = TextAnchor.MiddleRight;
            lAlias.style.flexGrow       = 1;
            lAlias.style.unityTextAlign = TextAnchor.MiddleCenter;

            fila.Add(lPos);
            fila.Add(lAlias);
            fila.Add(lPts);
            lista.Add(fila);
        }
    }

    void MostrarHistorico(ClickEvent evt)
    {
        if (panelSemanal   != null) panelSemanal.style.display   = DisplayStyle.None;
        if (panelHistorico != null) panelHistorico.style.display = DisplayStyle.Flex;
    }

    void MostrarSemanal(ClickEvent evt)
    {
        if (panelSemanal   != null) panelSemanal.style.display   = DisplayStyle.Flex;
        if (panelHistorico != null) panelHistorico.style.display = DisplayStyle.None;
    }

    void VolverMenu(ClickEvent evt) => SceneManager.LoadScene("MenuPrincipal");

    [System.Serializable]
    private class RankingEntry
    {
        public int    posicion;
        public string alias;
        public int    puntaje;
    }
}

public static class JsonHelper
{
    public static List<T> ParseList<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return new List<T>();
        string wrapped     = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper?.items != null ? new List<T>(wrapper.items) : new List<T>();
    }

    [System.Serializable]
    private class Wrapper<T> { public T[] items; }
}