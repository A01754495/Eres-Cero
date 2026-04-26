using UnityEngine;

public class NumeroDisplay : MonoBehaviour
{
    public NumeroAnimado[] digitos;

    [Header("Signo menos — arrastra aquí el GameObject con menos.png")]
    public SpriteRenderer signoMenos;

    [Header("Separación entre dígitos")]
    public float separacion = 1.5f;

    [Header("Distancia extra del signo menos al primer dígito")]
    public float offsetSignoMenos = 1.4f; 

    public void MostrarNumero(int valor)
    {
        string texto    = Mathf.Abs(valor).ToString();
        int    offset   = digitos.Length - texto.Length;
        bool   esNegativo = valor < 0;

        // Calcular posición inicial centrada
        float inicio = -(texto.Length - 1) * separacion / 2f;

        // Mostrar/ocultar y posicionar el signo menos
        if (signoMenos != null)
        {
            signoMenos.gameObject.SetActive(esNegativo);
            if (esNegativo)
            {
                // Se coloca justo a la izquierda del primer dígito
                float xPrimerDigito = inicio;
                signoMenos.transform.localPosition = new Vector3(
                    xPrimerDigito - offsetSignoMenos, 0, 0);
            }
        }

        for (int i = 0; i < digitos.Length; i++)
        {
            if (i < offset)
            {
                digitos[i].gameObject.SetActive(false);
            }
            else
            {
                digitos[i].gameObject.SetActive(true);

                int indexTexto = i - offset;
                int num = int.Parse(texto[indexTexto].ToString());

                digitos[i].SetNumero(num);
                digitos[i].transform.localPosition = new Vector3(
                    inicio + indexTexto * separacion, 0, 0);
            }
        }
    }

    public void CambiarColor(Color color)
    {
        foreach (var digito in digitos)
        {
            var sr = digito.GetComponent<SpriteRenderer>();
            if (sr != null && sr.material != null)
                sr.material.SetColor("_Color", color);
        }

        if (signoMenos != null)
            signoMenos.material.color = color;
    }
}