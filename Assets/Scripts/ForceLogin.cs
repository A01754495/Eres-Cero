using UnityEngine;
using UnityEngine.SceneManagement;

public static class ForceLogin
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        //  Revisar si ya inició sesión
        int logged = PlayerPrefs.GetInt("LoggedIn", 0);

        //  Si NO ha iniciado sesión
        if (logged == 0)
        {
            //  Evitar loop si ya está en Login
            if (SceneManager.GetActiveScene().name != "Login")
            {
                SceneManager.LoadScene("Login");
            }
        }
    }
}