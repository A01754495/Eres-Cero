using UnityEngine;
using UnityEngine.UIElements;

public class SkinSelector : MonoBehaviour
{
    // Lista de colores (puedes agregar más)
    private Color[] colores = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f),   // naranja
        new Color(0.6f, 0.2f, 1f), // morado
        new Color(1f, 0.8f, 0.2f), // dorado
        Color.white,
        Color.gray,
        Color.black
    };

    private Image preview;
    private int skinSeleccionada = 0;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        preview = root.Q<Image>("ImagenPreview");

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

        root.Q<Button>("BtnSeleccionar").clicked += ConfirmarSkin;

        // cargar skin guardada
        skinSeleccionada = PlayerPrefs.GetInt("SkinSeleccionada", 0);
        CambiarSkin(skinSeleccionada);
    }

    void CambiarSkin(int index)
    {
        skinSeleccionada = index;

        // 🔥 preview
        preview.style.backgroundColor = new StyleColor(colores[index]);

        // 🔥 aplicar al jugador
        AplicarColor(colores[index]);
    }

    void ConfirmarSkin()
    {
        PlayerPrefs.SetInt("SkinSeleccionada", skinSeleccionada);
        Debug.Log("Skin guardada: " + skinSeleccionada);
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