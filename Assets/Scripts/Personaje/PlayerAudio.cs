using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Audios")]
    public AudioClip sonidoPuntos;
    public AudioClip sonidoPerder;
    public AudioClip sonidoMover;

    private bool reproduciendoMovimiento = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirPuntos()
    {
        audioSource.PlayOneShot(sonidoPuntos);
    }

    public void ReproducirPerder()
    {
        audioSource.PlayOneShot(sonidoPerder);
    }

    public void ReproducirMovimiento(bool mover)
    {
        if (mover && !reproduciendoMovimiento)
        {
            audioSource.clip = sonidoMover;
            audioSource.loop = true;
            audioSource.Play();
            reproduciendoMovimiento = true;
        }
        else if (!mover && reproduciendoMovimiento)
        {
            audioSource.Stop();
            audioSource.loop = false;
            reproduciendoMovimiento = false;
        }
    }
}