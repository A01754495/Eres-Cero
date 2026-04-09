using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameplayController : MonoBehaviour
{
    private UIDocument ui;
    private Button btnPerder;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        btnPerder = root.Q<Button>("BtnPerder");

        if (btnPerder != null)
            btnPerder.RegisterCallback<ClickEvent>(IrAGameOver);
    }

    void OnDisable()
    {
        if (btnPerder != null)
            btnPerder.UnregisterCallback<ClickEvent>(IrAGameOver);
    }

    void IrAGameOver(ClickEvent evt)
    {
        SceneManager.LoadScene("GameOver");
    }
}