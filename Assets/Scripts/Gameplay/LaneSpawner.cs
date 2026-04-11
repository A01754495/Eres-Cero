using UnityEngine;
using System.Collections.Generic;

public class LaneSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabCasilla;
    public GameObject prefabPuerta;

    [Header("Posiciones X de los 3 carriles")]
    public float carrilIzquierda = -6.4f;
    public float carrilCentro   =  0f;
    public float carrilDerecha  =  6.4f;

    [Header("Coordenadas Y de spawn y destrucción")]
    public float spawnY   =  6.4f;
    public float destroyY = -6.4f;

    [Header("Distancia vertical entre la casilla y la puerta")]
    public float distanciaPuerta = 2.7f;

    private float[] carriles;

    private string[] opsFacil   = { "+", "-" };
    private string[] opsMedio   = { "+", "-", "*" };
    private string[] opsDificil = { "+", "-", "*", "/" };

    void Start()
    {
        carriles = new float[] { carrilIzquierda, carrilCentro, carrilDerecha };
        SpawnOla();
    }

    void Update()
    {
        float velocidad = GameManager.Instance != null
                        ? GameManager.Instance.VelocidadActual()
                        : 3f;

        foreach (Transform hijo in transform)
            hijo.position += Vector3.down * velocidad * Time.deltaTime;

        List<Transform> aEliminar = new List<Transform>();
        foreach (Transform hijo in transform)
        {
            if (hijo.position.y < destroyY)
                aEliminar.Add(hijo);
        }

        foreach (var h in aEliminar)
            Destroy(h.gameObject);
    }

    void SpawnOla()
    {
        int rango    = GameManager.Instance != null ? GameManager.Instance.RangoOperacion : 5;
        string[] ops = ObtenerOperadores();

        //  La base real es el valor actual del jugador,
        // no una meta futura generada antes de tiempo
        int valorBase = GameManager.Instance != null ? GameManager.Instance.ValorJugador : 0;

        if (GameManager.Instance != null)
            GameManager.Instance.ValorBase = valorBase;

        int meta;
        int intentosMeta = 0;
        do
        {
            meta = Random.Range(1, rango + 1);
            intentosMeta++;
        }
        while (meta == valorBase && intentosMeta < 50);

        if (meta == valorBase)
            meta = valorBase + 1;

        int diferencia     = meta - valorBase;
        int carrilSolucion = Random.Range(0, 3);

        string[] opsCarril  = new string[3];
        int[]    numsCarril = new int[3];
        int[]    resultados = new int[3];

        for (int i = 0; i < 3; i++)
        {
            if (i == carrilSolucion)
            {
                opsCarril[i]  = diferencia >= 0 ? "+" : "-";
                numsCarril[i] = Mathf.Abs(diferencia);
                resultados[i] = meta;
            }
            else
            {
                string opDist;
                int numDist;
                int resultadoDist;
                int intentos = 0;

                do
                {
                    opDist  = ops[Random.Range(0, ops.Length)];
                    numDist = Random.Range(1, rango + 1);

                    resultadoDist = opDist switch
                    {
                        "+" => valorBase + numDist,
                        "-" => valorBase - numDist,
                        "*" => valorBase * numDist,
                        "/" => numDist != 0 ? valorBase / numDist : valorBase + 1,
                        _   => valorBase + 1
                    };

                    intentos++;
                }
                while (resultadoDist == meta && intentos < 20);

                opsCarril[i]  = opDist;
                numsCarril[i] = numDist;
                resultados[i] = resultadoDist;
            }
        }

        Shuffle(ref opsCarril, ref numsCarril, ref resultados);

        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(prefabCasilla, transform);
            go.transform.position = new Vector3(carriles[i], spawnY, 0f);

            CasillaOperacion casilla = go.GetComponent<CasillaOperacion>();
            if (casilla != null)
            {
                casilla.resultadoFinal = resultados[i];
                casilla.Configurar(opsCarril[i], numsCarril[i]);
            }
        }

        GameObject puertaGO = Instantiate(prefabPuerta, transform);
        puertaGO.transform.position = new Vector3(carrilCentro, spawnY + distanciaPuerta, 0f);

        PuertaController puerta = puertaGO.GetComponent<PuertaController>();
        if (puerta != null)
            puerta.Configurar(meta);
    }

    public void ForzarNuevaOla()
    {
        // genera una nueva ola solo cuando pasas la puerta correcta
        SpawnOla();
    }

    string[] ObtenerOperadores()
    {
        if (GameManager.Instance == null) return opsFacil;

        return GameManager.Instance.Dificultad switch
        {
            "medio"   => opsMedio,
            "dificil" => opsDificil,
            _         => opsFacil,
        };
    }

    void Shuffle(ref string[] ops, ref int[] nums, ref int[] res)
    {
        for (int i = ops.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            string tOp = ops[i];
            ops[i] = ops[j];
            ops[j] = tOp;

            int tNum = nums[i];
            nums[i] = nums[j];
            nums[j] = tNum;

            int tRes = res[i];
            res[i] = res[j];
            res[j] = tRes;
        }
    }
}