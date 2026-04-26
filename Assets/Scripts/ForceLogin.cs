using UnityEngine;
using UnityEngine.SceneManagement;

public static class ForceLogin
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        // Si ya hay sesión guardada en disco, no hace falta ir al Login
        int idGuardado = PlayerPrefs.GetInt("SesionIdJugador", -1);
        if (idGuardado > 0) return;

        // Sin sesión guardada → ir a Login (si no está ya ahí)
        if (SceneManager.GetActiveScene().name != "Login")
            SceneManager.LoadScene("Login");
    }
}