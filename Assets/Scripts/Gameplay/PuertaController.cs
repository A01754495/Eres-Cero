using UnityEngine;
using TMPro;

// PuertaController vive en cada prefab de puerta.
// El LaneSpawner la configura al instanciarla.

public class PuertaController : MonoBehaviour
{
    [HideInInspector] public int numeroMeta;

    public TextMeshPro textoMeta;       // muestra el número que el jugador debe igualar
    public SpriteRenderer spriteRend;   // para feedback de color

    public void Configurar(int meta)
    {
        numeroMeta = meta;
        if (textoMeta != null)
            textoMeta.text = meta.ToString();
    }

    public void MostrarExito()
    {
        if (spriteRend != null)
            spriteRend.color = Color.green;
        // La puerta se destruye poco después para no bloquear
        Destroy(gameObject, 0.5f);
    }

    public void MostrarError()
    {
        if (spriteRend != null)
            spriteRend.color = Color.red;
        // No la destruimos; el GameOver se carga en 0.8s desde PlayerController
    }
}
