using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Qué tan rápido se mueve el fondo vs los objetos del juego")]
    [Range(0.1f, 1f)]
    public float velocidadMultiplicador = 0.4f;

    [Header("Altura del sprite en unidades de mundo")]
    public float alturaSprite = 11f; // ajusta según el tamaño real de tu sprite

    private Transform copia; // segunda copia del fondo

    void Start()
    {
        // Crear una copia del fondo y ponerla justo arriba
        GameObject go = new GameObject("FondoCopia");
        go.transform.parent = transform.parent;

        SpriteRenderer srOriginal = GetComponent<SpriteRenderer>();
        SpriteRenderer srCopia    = go.AddComponent<SpriteRenderer>();

        srCopia.sprite       = srOriginal.sprite;
        srCopia.sortingOrder = srOriginal.sortingOrder;
        srCopia.sortingLayerID = srOriginal.sortingLayerID;
        srCopia.drawMode     = srOriginal.drawMode;
        srCopia.size         = srOriginal.size;
        srCopia.color        = srOriginal.color;

        // Posicionar la copia exactamente arriba del original
        go.transform.localScale = transform.localScale;
        go.transform.position   = new Vector3(
            transform.position.x,
            transform.position.y + alturaSprite,
            transform.position.z
        );

        copia = go.transform;
    }

    void Update()
    {
        float velocidad = GameManager.Instance != null
                        ? GameManager.Instance.VelocidadActual() * velocidadMultiplicador
                        : 1.5f;

        // Mover ambos hacia abajo
        transform.position  += Vector3.down * velocidad * Time.deltaTime;
        copia.position      += Vector3.down * velocidad * Time.deltaTime;

        // Si el original salió por abajo, lo teletransportamos arriba de la copia
        if (transform.position.y <= -alturaSprite)
            transform.position = new Vector3(
                transform.position.x,
                copia.position.y + alturaSprite,
                transform.position.z
            );

        // Si la copia salió por abajo, la teletransportamos arriba del original
        if (copia.position.y <= -alturaSprite)
            copia.position = new Vector3(
                copia.position.x,
                transform.position.y + alturaSprite,
                transform.position.z
            );
    }
}