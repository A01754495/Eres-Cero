using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RankingsController : MonoBehaviour
{
    private UIDocument ui;

    private VisualElement panelSemanal;
    private VisualElement panelHistorico;

    private Button btnVerHistorico;
    private Button btnVerSemanal;
    private Button btnVolverHistorico;
    private Button btnVolverSemanal;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        panelSemanal = root.Q<VisualElement>("PanelSemanal");
        panelHistorico = root.Q<VisualElement>("PanelHistorico");

        btnVerHistorico = root.Q<Button>("BtnVerHistorico");
        btnVerSemanal = root.Q<Button>("BtnVerSemanal");
        btnVolverHistorico = root.Q<Button>("BtnVolverHistorico");
        btnVolverSemanal = root.Q<Button>("BtnVolverSemanal");

        panelSemanal.style.display = DisplayStyle.Flex;
        panelHistorico.style.display = DisplayStyle.None;

        if (btnVerHistorico != null)
            btnVerHistorico.RegisterCallback<ClickEvent>(MostrarHistorico);

        if (btnVerSemanal != null)
            btnVerSemanal.RegisterCallback<ClickEvent>(MostrarSemanal);

        if (btnVolverHistorico != null)
            btnVolverHistorico.RegisterCallback<ClickEvent>(VolverMenu);

        if (btnVolverSemanal != null)
            btnVolverSemanal.RegisterCallback<ClickEvent>(VolverMenu);
    }

    void OnDisable()
    {
        if (btnVerHistorico != null)
            btnVerHistorico.UnregisterCallback<ClickEvent>(MostrarHistorico);

        if (btnVerSemanal != null)
            btnVerSemanal.UnregisterCallback<ClickEvent>(MostrarSemanal);

        if (btnVolverHistorico != null)
            btnVolverHistorico.UnregisterCallback<ClickEvent>(VolverMenu);

        if (btnVolverSemanal != null)
            btnVolverSemanal.UnregisterCallback<ClickEvent>(VolverMenu);
    }

    void MostrarHistorico(ClickEvent evt)
    {
        panelSemanal.style.display = DisplayStyle.None;
        panelHistorico.style.display = DisplayStyle.Flex;
    }

    void MostrarSemanal(ClickEvent evt)
    {
        panelSemanal.style.display = DisplayStyle.Flex;
        panelHistorico.style.display = DisplayStyle.None;
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}