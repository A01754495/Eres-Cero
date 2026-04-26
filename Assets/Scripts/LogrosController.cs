using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LogrosController : MonoBehaviour
{
    private UIDocument ui;
    private Button        btnVolver;
    private VisualElement contenedorLogros;
    private Label         labelSinDatos;

    // Misma referencia que las skins usan
    private VisualElement[] logros = new VisualElement[6];

    // Condiciones de desbloqueo por logro (índice 0 = Logro1, etc.)
    // Devuelve true si el logro está desbloqueado
    bool[] VerificarDesbloqueos()
    {
        int puntajeTotal  = PlayerPrefs.GetInt("PuntajeTotal", 0);
        int puertasTotales = PlayerPrefs.GetInt("PuertasTotales", 0);
        int skinsUsadas   = PlayerPrefs.GetInt("SkinsUsadas", 0);

        return new bool[]
        {
            // Logro1 — Hardcore protocol: 1000 puertas en difícil
            PlayerPrefs.GetInt("PuertasDificil", 0) >= 1000,

            // Logro2 — Identidad Completa: usar todas las skins (12)
            skinsUsadas >= 12,

            // Logro3 — Ambición creciente: 400 puntos en una sola sesión
            PlayerPrefs.GetInt("MejorPuntaje", 0) >= 400,

            // Logro4 — Travesía infinita: 1000 puertas en total
            puertasTotales >= 1000,

            // Logro5 — Dominio absoluto: top 3 por 30 días (muy difícil — se deja bloqueado por ahora)
            false,

            // Logro6 — Overflow: 1,000,000 puntos
            puntajeTotal >= 1000000,
        };
    }

    void OnEnable()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null) return;
        var root = ui.rootVisualElement;

        btnVolver        = root.Q<Button>("BtnVolver");
        contenedorLogros = root.Q<VisualElement>("ContenedorLogros");
        labelSinDatos    = root.Q<Label>("LabelSinDatos");

        // Buscar los 6 logros por nombre
        for (int i = 0; i < 6; i++)
            logros[i] = root.Q<VisualElement>($"Logro{i + 1}");

        if (btnVolver != null)
            btnVolver.RegisterCallback<ClickEvent>(e =>
            {
                UISoundManager.Instance.PlayClick();
                VolverMenu(e);
            });

        VerificarPartidas();
        AplicarEstadosLogros();
    }

    void OnDisable() { }

    void VerificarPartidas()
    {
        bool haJugado = PlayerPrefs.GetInt("HaJugado", 0) == 1;

        if (!haJugado)
        {
            if (contenedorLogros != null)
                contenedorLogros.style.display = DisplayStyle.None;
            if (labelSinDatos != null)
                labelSinDatos.style.display = DisplayStyle.Flex;
        }
        else
        {
            if (contenedorLogros != null)
                contenedorLogros.style.display = DisplayStyle.Flex;
            if (labelSinDatos != null)
                labelSinDatos.style.display = DisplayStyle.None;
        }
    }

    void AplicarEstadosLogros()
    {
        bool[] desbloqueados = VerificarDesbloqueos();

        for (int i = 0; i < 6; i++)
        {
            if (logros[i] == null) continue;

            if (desbloqueados[i])
            {
                // Desbloqueado — se ve normal
                logros[i].style.opacity = 1f;
            }
            else
            {
                // Bloqueado — gris igual que las skins
                logros[i].style.opacity = 0.35f;
            }
        }
    }

    void VolverMenu(ClickEvent evt)
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}