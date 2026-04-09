using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LogrosController : MonoBehaviour
{
    private UIDocument ui;
    private Button btnVolver;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        btnVolver = root.Q<Button>("BtnVolver");

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(VolverMenu);
    }

    void OnDisable()
    {
        if (btnVolver != null)
            btnVolver.UnregisterCallback<ClickEvent>(VolverMenu);
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}