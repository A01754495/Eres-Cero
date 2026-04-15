using UnityEngine;

public class NumeroDisplay : MonoBehaviour
{
    public NumeroAnimado[] digitos;
    
    public void MostrarNumero(int valor)
    {
        string texto = Mathf.Abs(valor).ToString(); 

        int offset = digitos.Length - texto.Length;

        float separacion = 1.5f;
        float inicio = -(texto.Length - 1) * separacion / 2f;

        for (int i = 0; i < digitos.Length; i++)
        {
            if (i < offset)
            {
                // ocultar dígitos sobrantes
                digitos[i].gameObject.SetActive(false);
            }
            else
            {
                digitos[i].gameObject.SetActive(true);

                int indexTexto = i - offset;
                int num = int.Parse(texto[indexTexto].ToString());

                digitos[i].SetNumero(num);
                digitos[i].transform.localPosition = new Vector3(inicio + indexTexto * separacion, 0, 0);
            }
        }

    for (int i = 0; i < texto.Length; i++)
    {
        digitos[offset + i].transform.localPosition =
            new Vector3(inicio + i * separacion, 0, 0);
    }
    }

    public void CambiarColor(Color color)
    {
        foreach (var digito in digitos)
        {
            var sr = digito.GetComponent<SpriteRenderer>();

            if (sr != null && sr.material != null)
            {
                sr.material.SetColor("_Color", color);
            }
        }
    }
}