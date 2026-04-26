using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RetroalimentacionController : MonoBehaviour
{
    private UIDocument ui;
    private Label  labelRetroalimentacion;
    private Button btnContinuar;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        labelRetroalimentacion = root.Q<Label>("LabelRetroalimentacion");
        btnContinuar           = root.Q<Button>("BtnContinuar");

        MostrarRetroalimentacion();

        if (btnContinuar != null)
            btnContinuar.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance?.PlayClick();
                SceneManager.LoadScene("GameOver");
            });
    }

    void MostrarRetroalimentacion()
    {
        if (GameManager.Instance == null || labelRetroalimentacion == null) return;

        var    gm    = GameManager.Instance;
        string op    = gm.UltimoOperador;
        int    num   = gm.UltimoNumero;
        int    res   = gm.UltimoResultado;
        int    meta  = gm.MetaFallida;
        int    base_ = gm.UltimoValorBase;

        // Línea 1: la operación que hizo el jugador
        string linea1 = !string.IsNullOrEmpty(op) && num != 0
            ? $"{base_} {op} {num} = {res}"
            : $"Tu número era {res}";

        // Línea 2: consejo simple y siempre correcto
        string linea2 = $"La puerta pedía {meta}.\n" + SugerirOperacion(base_, meta);

        labelRetroalimentacion.text = $"{linea1}\n{linea2}";
    }

    // Sugiere la operación más simple para llegar de base a meta
    string SugerirOperacion(int base_, int meta)
    {
        if (base_ == 0)
            return $"Necesitabas sumar {meta} para llegar a {meta}.";

        int diff = meta - base_;

        // ¿Se puede multiplicar exactamente?
        if (base_ != 0 && meta % base_ == 0 && meta / base_ > 1)
            return $"Pista: {base_} x {meta / base_} = {meta}.";

        // ¿Se puede dividir exactamente?
        if (base_ != 0 && base_ % meta == 0 && base_ / meta > 1)
            return $"Pista: {base_} ÷ {base_ / meta} = {meta}.";

        // Suma o resta
        if (diff > 0)
            return $"Pista: {base_} + {diff} = {meta}.";
        else
            return $"Pista: {base_} - {Mathf.Abs(diff)} = {meta}.";
    }
}