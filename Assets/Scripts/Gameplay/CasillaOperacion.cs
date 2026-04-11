using UnityEngine;
using TMPro;

// CasillaOperacion vive en cada GameObject de tipo "casilla".
// El LaneSpawner la configura al instanciarla.

public class CasillaOperacion : MonoBehaviour
{
    [HideInInspector] public string operador; // "+", "-", "*", "/"
    [HideInInspector] public int    numero;
    [HideInInspector] public int    resultadoFinal;

    public TextMeshPro textoOperacion; // arrastra el TMP en el Inspector del prefab

    // El Spawner llama esto justo después de instanciar
    public void Configurar(string op, int num)
    {
        operador = op;
        numero   = num;

        if (textoOperacion != null)
            textoOperacion.text = op + num.ToString();
    }
}
