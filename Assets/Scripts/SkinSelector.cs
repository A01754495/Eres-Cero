using UnityEngine;
using UnityEngine.UIElements;

public class SkinSelector : MonoBehaviour
{
    public Texture2D[] skins; // arrastras tus 12 imágenes aquí

    private Image preview;
    private int skinSeleccionada = 0;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Imagen grande
        preview = root.Q<Image>("ImagenPreview");

        // Botones
        for (int i = 0; i < skins.Length; i++)
        {
            int index = i; // IMPORTANTE (bug común)
            var boton = root.Q<Button>("BtnSkin" + (i + 1));

            boton.clicked += () =>
            {
                CambiarSkin(index);
            };
        }

        // Botón seleccionar
        root.Q<Button>("BtnSeleccionar").clicked += ConfirmarSkin;
    }

    void CambiarSkin(int index)
    {
        skinSeleccionada = index;

        preview.style.backgroundImage = new StyleBackground(skins[index]);
    }

    void ConfirmarSkin()
    {
        PlayerPrefs.SetInt("SkinSeleccionada", skinSeleccionada);
        Debug.Log("Skin guardada: " + skinSeleccionada);
    }
}