using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";
    private UIDocument ui;
    private Label  labelPuntajeFinal;
    private Label  labelPuertas;
    private Label  labelDificultad;
    private Label  labelTiempo;
    private Button btnReintentar;
    private Button btnMenu;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        labelPuntajeFinal = root.Q<Label>("LabelPuntajeFinal");
        labelPuertas      = root.Q<Label>("LabelPuertas");
        labelDificultad   = root.Q<Label>("LabelDificultad");
        labelTiempo       = root.Q<Label>("LabelTiempo");
        btnReintentar     = root.Q<Button>("BtnReintentar");
        btnMenu           = root.Q<Button>("BtnMenu");

        if (GameManager.Instance != null)
        {
            // Acumular puntaje histórico local (para desbloqueo de aspectos)
            GameManager.Instance.AcumularPuntaje();

            if (labelPuntajeFinal != null)
                labelPuntajeFinal.text = "PUNTAJE: " + GameManager.Instance.Puntaje;
            if (labelPuertas != null)
                labelPuertas.text = "Puertas cruzadas: " + GameManager.Instance.PuertasVivas;
            if (labelDificultad != null)
                labelDificultad.text = "Dificultad: " + GameManager.Instance.Dificultad.ToUpper();
            if (labelTiempo != null)
            {
                int seg = Mathf.FloorToInt(GameManager.Instance.TiempoPartida);
                labelTiempo.text = $"Tiempo: {seg / 60:00}:{seg % 60:00}";
            }

            if (GameManager.Instance.HaySesion)
                StartCoroutine(GuardarPartida());
        }

            if (btnReintentar != null)
                btnReintentar.RegisterCallback<ClickEvent>(e =>
                {
                    UISoundManager.Instance.PlayClick(); //  sonido
                    OnReintentar(e);                     // acción 
                });

            if (btnMenu != null)
                btnMenu.RegisterCallback<ClickEvent>(e =>
                {
                    UISoundManager.Instance.PlayClick(); //  sonido
                    OnMenu(e);                           // acción 
                });
    }

    void OnDisable()
    {
        //  No desregistramos callbacks porque usamos lambdas
        //if (btnReintentar != null) btnReintentar.UnregisterCallback<ClickEvent>(OnReintentar);
        //if (btnMenu       != null) btnMenu.UnregisterCallback<ClickEvent>(OnMenu);
    }

    void OnReintentar(ClickEvent e)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarPartida(GameManager.Instance.Dificultad);
        SceneManager.LoadScene("Dificultad");
    }

    void OnMenu(ClickEvent e) => SceneManager.LoadScene("MenuPrincipal");

    IEnumerator GuardarPartida()
    {
        var gm = GameManager.Instance;

        string fechaHora = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string tiempo = gm.TiempoPartida.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        string body = $"{{\"idJugador\":{gm.IdJugador}," +
                    $"\"fechaHora\":\"{fechaHora}\"," +
                    $"\"puntaje\":{gm.Puntaje}," +
                    $"\"dificultad\":\"{gm.Dificultad}\"," +
                    $"\"tiempo\":{tiempo}}}"; 

        using var req = new UnityWebRequest($"{URL_BASE}/partida", "POST");
        req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogWarning("No se pudo guardar la partida: " +req.downloadHandler.text);
        else
            Debug.Log("Partida guardada: " + req.downloadHandler.text);
    }
}