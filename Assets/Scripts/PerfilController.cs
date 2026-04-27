using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class PerfilController : MonoBehaviour
{
    private const string URL_BASE = "https://ygtfxb3dtnzrhhgw4sixxcynsq0qnzpw.lambda-url.us-east-1.on.aws";

    private UIDocument ui;

    private Label  labelUsuario;
    private Label  labelPuntaje;
    private Button btnVolver;
    private Button btnCerrarSesion;

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        labelUsuario    = root.Q<Label>("Usuario");
        labelPuntaje    = root.Q<Label>("Puntaje");
        btnVolver       = root.Q<Button>("BtnVolver");
        btnCerrarSesion = root.Q<Button>("BtnCerrarSesion");

        if (labelUsuario != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.HaySesion)
                labelUsuario.text = "Nombre de usuario: " + GameManager.Instance.AliasJugador;
            else
                labelUsuario.text = "Nombre de usuario: Invitado";
        }

        // Mostrar "Cargando..." mientras trae el puntaje
        if (labelPuntaje != null)
            labelPuntaje.text = "Puntaje total: ...";

        if (GameManager.Instance != null && GameManager.Instance.HaySesion)
            StartCoroutine(CargarPuntajeTotal());

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                VolverMenu(e);
            });

        if (btnCerrarSesion != null)
            btnCerrarSesion.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                CerrarSesion(e);
            });
    }

    void OnDisable() { }

    IEnumerator CargarPuntajeTotal()
    {
        int idJugador = GameManager.Instance.IdJugador;
        using var req = UnityWebRequest.Get($"{URL_BASE}/puntaje-total/{idJugador}");
        yield return req.SendWebRequest();

        if (labelPuntaje == null) yield break;

        if (req.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<RespuestaPuntaje>(req.downloadHandler.text);
            int puntaje = resp != null ? resp.puntajeTotal : 0;
            labelPuntaje.text = $"Puntaje total: {puntaje}";
        }
        else
        {
            labelPuntaje.text = "Puntaje total: --";
            Debug.LogWarning("No se pudo cargar puntaje total: " + req.downloadHandler.text);
        }
    }

    void VolverMenu(ClickEvent evt) => SceneManager.LoadScene("MenuPrincipal");

    void CerrarSesion(ClickEvent evt)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.CerrarSesion();
        SceneManager.LoadScene("Login");
    }

    [System.Serializable] private class RespuestaPuntaje { public int puntajeTotal; }
}