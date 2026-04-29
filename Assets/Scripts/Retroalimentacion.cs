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

        var gm   = GameManager.Instance;
        int res  = gm.ValorJugador;
        int meta = gm.MetaFallida;
        int base_ = gm.ValorInicioOla; // valor al inicio de la ola

        string op  = gm.UltimoOperador;
        int    num = gm.UltimoNumero;

        // Línea 1: lo que hizo en la última casilla
        string linea1 = !string.IsNullOrEmpty(op) && num != 0
            ? $"{gm.UltimoValorBase} {op} {num} = {res}"
            : $"Tu número era {res}";

        // Línea 2: pista desde el inicio de la ola
        string linea2;

        linea2 = $"La puerta pedía {meta}.\n" + SugerirOperacion(base_, meta);

        labelRetroalimentacion.text = $"{linea1}\n{linea2}";
        Debug.Log($"ValorInicioOla:{gm.ValorInicioOla} UltimoValorBase:{gm.UltimoValorBase} op:{op} num:{num} res:{res} meta:{meta}");
    }
    // Sugiere la operación más simple para llegar de base a meta
    string SugerirOperacion(int base_, int meta)
    {
        if (base_ == 0)
            return $"Necesitabas llegar a {meta}.";

        if (meta == 0)
            return $"Pista: {base_} - {base_} = 0.";

        int diff = meta - base_;

        // ¿Se puede multiplicar exactamente?
        if (meta % base_ == 0 && meta / base_ > 1)
            return $"Pista: {base_} x {meta / base_} = {meta}.";

        // ¿Se puede dividir exactamente?
        if (meta != 0 && base_ % meta == 0 && base_ / meta > 1)
            return $"Pista: {base_} ÷ {base_ / meta} = {meta}.";

        // Suma o resta
        if (diff > 0)
            return $"Pista: {base_} + {diff} = {meta}.";
        else
            return $"Pista: {base_} - {Mathf.Abs(diff)} = {meta}.";
    }
}