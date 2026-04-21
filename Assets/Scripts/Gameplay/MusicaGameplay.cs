using UnityEngine;

public class MusicaGameplay : MonoBehaviour
{
    private AudioSource musica;

    void Start()
    {
        musica = GetComponent<AudioSource>();

        if (musica != null)
        {
            musica.Play();
        }
    }
}