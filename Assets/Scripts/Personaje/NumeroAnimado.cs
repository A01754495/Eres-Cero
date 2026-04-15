using UnityEngine;

public class NumeroAnimado : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    private int numeroActual = 0;
    private int frameActual = 0;
    private float timer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // crear material único para cada dígito
        spriteRenderer.material = new Material(spriteRenderer.material);
    }

    public void SetNumero(int numero)
    {
        // spriteRenderer.material.SetColor("_Color", Color.red); // cambiar color (prueba)
        numeroActual = numero;
        if (numeroActual == 1)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 0.2f)
        {
            frameActual = 1 - frameActual;

            int index = numeroActual * 2 + frameActual;

            if (index < sprites.Length)
                spriteRenderer.sprite = sprites[index];

            timer = 0f;
        }
    }
}