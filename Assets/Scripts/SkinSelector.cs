using UnityEngine;
using UnityEngine.UIElements;

public class SkinSelector : MonoBehaviour
{
    // COLORES (LÓGICA)
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

    // IMÁGENES (VISUAL)
    public Texture2D[] skins;

    private VisualElement preview;
    private int skinSeleccionada = 0;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        preview = root.Q<VisualElement>("ImagenPreview");

        // NO registrar callbacks de botones aquí — los maneja AspectosController

        // Cargar skin desde GameManager
        skinSeleccionada = GameManager.Instance != null ? GameManager.Instance.SkinSeleccionada : 0;
        CambiarSkin(skinSeleccionada);
    }

    // Public — AspectosController lo llama al hacer clic en una skin
    public void CambiarSkin(int index)
    {
        if (index < 0 || index >= colores.Length) index = 0;
        skinSeleccionada = index;

        // Aplicar color al personaje
        AplicarColor(colores[index]);

        // Actualizar imagen de preview
        if (preview == null) return;

        if (skins != null && index < skins.Length && skins[index] != null)
        {
            preview.style.backgroundImage = new StyleBackground(skins[index]);
            preview.style.backgroundColor = Color.clear;
        }
        else
        {
            preview.style.backgroundImage = null;
            preview.style.backgroundColor = new StyleColor(colores[index]);
        }
    }

    void AplicarColor(Color color)
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        var renderers = player.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
            if (sr.material != null)
                sr.material.SetColor("_Color", color);
    }
}