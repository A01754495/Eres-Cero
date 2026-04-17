using UnityEngine;
using UnityEngine.UIElements;

public class SkinSelector : MonoBehaviour
{
    //  COLORES (LÓGICA) 
    private Color[] colores = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f),
        new Color(0.6f, 0.2f, 1f),
        new Color(1f, 0.8f, 0.2f),
        Color.white,
        Color.gray,
        Color.black
    };

    //  IMÁGENES (VISUAL) 
    public Texture2D[] skins;

    private VisualElement preview;
    private int skinSeleccionada = 0;

    private Button btnSeleccionar;
    private Button btnSeleccionado;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        preview = root.Q<VisualElement>("ImagenPreview");

        // BOTONES SKIN
        for (int i = 0; i < colores.Length; i++)
        {
            int index = i;
            var boton = root.Q<Button>("BtnSkin" + (i + 1));

            if (boton != null)
            {
                boton.clicked += () =>
                {
                    CambiarSkin(index);
                };
            }
        }

        // BOTONES
        btnSeleccionar = root.Q<Button>("BtnSeleccionar");
        btnSeleccionado = root.Q<Button>("BtnSeleccionado");

        btnSeleccionado.style.display = DisplayStyle.None;

        btnSeleccionar.clicked += ConfirmarSkin;

        // cargar skin guardada
        skinSeleccionada = PlayerPrefs.GetInt("SkinSeleccionada", 0);
        CambiarSkin(skinSeleccionada);
    }

    void CambiarSkin(int index)
    {
        skinSeleccionada = index;

        //  LÓGICA → SIEMPRE COLOR 
        AplicarColor(colores[index]);

        //  VISUAL → IMAGEN SI EXISTE 
        if (skins != null && index < skins.Length && skins[index] != null)
        {
            preview.style.backgroundImage = new StyleBackground(skins[index]);
            preview.style.backgroundColor = Color.clear;
        }
        else
        {
            // fallback si no hay imagen
            preview.style.backgroundImage = null;
            preview.style.backgroundColor = new StyleColor(colores[index]);
        }
    }

    void ConfirmarSkin()
    {
        PlayerPrefs.SetInt("SkinSeleccionada", skinSeleccionada);

        btnSeleccionar.style.display = DisplayStyle.None;
        btnSeleccionado.style.display = DisplayStyle.Flex;
    }

    void AplicarColor(Color color)
    {
        var player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            var renderers = player.GetComponentsInChildren<SpriteRenderer>();

            foreach (var sr in renderers)
            {
                if (sr.material != null)
                {
                    sr.material.SetColor("_Color", color);
                }
            }
        }
    }
}