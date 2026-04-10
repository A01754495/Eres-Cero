using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AspectosController : MonoBehaviour
{
    private UIDocument ui;

    private Button btnSeleccionar;
    private Button btnVolver;

    private VisualElement imagenPreview;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        // BOTONES
        btnSeleccionar = root.Q<Button>("BtnSeleccionar");
        btnVolver = root.Q<Button>("BtnVolver");

        // PREVIEW
        imagenPreview = root.Q<VisualElement>("ImagenPreview");

        // CALLBACKS
        if (btnSeleccionar != null)
            btnSeleccionar.RegisterCallback<ClickEvent>(SeleccionarSkin);

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(VolverMenu);
    }

    void OnDisable()
    {
        if (btnSeleccionar != null)
            btnSeleccionar.UnregisterCallback<ClickEvent>(SeleccionarSkin);

        if (btnVolver != null)
            btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);
    }

    //  BOTÓN SELECCIONAR
    void SeleccionarSkin(ClickEvent evt)
    {
        // cambia texto
        btnSeleccionar.text = "✔ Seleccionado";

        //  borde verde a preview
        imagenPreview.style.borderTopWidth = 4;
        imagenPreview.style.borderBottomWidth = 4;
        imagenPreview.style.borderLeftWidth = 4;
        imagenPreview.style.borderRightWidth = 4;

        imagenPreview.style.borderTopColor = Color.green;
        imagenPreview.style.borderBottomColor = Color.green;
        imagenPreview.style.borderLeftColor = Color.green;
        imagenPreview.style.borderRightColor = Color.green;


    }

    void RegresarMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    // BOTÓN VOLVER
    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}