using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Musica")]
    public AudioClip musicaMenu;
    public AudioClip musicaGameplay;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton (evita duplicados)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //  Obtener AudioSource SIEMPRE 
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource NO encontrado en AudioManager"); 
            return;
        }

        //  FORZAR PLAY DESDE AWAKE (esto reemplaza Play On Awake) 
        ReproducirMusica(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += AlCambiarEscena;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCambiarEscena;
    }

    void AlCambiarEscena(Scene scene, LoadSceneMode mode)
    {
        ReproducirMusica(scene.name);
    }

    void ReproducirMusica(string nombreEscena)
    {
        if (nombreEscena == "Cinematica")
        {
            audioSource.Stop();
            return;
        }
        
        AudioClip nuevaMusica;

        
        if (nombreEscena == "Gameplay")
            nuevaMusica = musicaGameplay;
        else
            nuevaMusica = musicaMenu;

        if (nuevaMusica == null)
        {
            Debug.LogWarning(" No hay música asignada para: " + nombreEscena);
            return;
        }

        // Si ya está sonando esa, no reiniciar
        if (audioSource.clip == nuevaMusica && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.loop = true;
        audioSource.volume = 1f;

        audioSource.Play();

        Debug.Log("🎵 Reproduciendo: " + nuevaMusica.name);
    }
}