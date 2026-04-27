using UnityEngine;
using System.Collections;
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
    public float spawnY   =  4f;
    public float destroyY = -6.4f;

    [Header("Distancia vertical entre casillas y puerta")]
    public float distanciaPuerta = 2.7f;

    [Header("Segundos que las casillas se muestran antes de bajar")]
    public float tiempoEspera = 2f;

    private float[] carriles;

    // Lista de olas activas — cada ola tiene sus objetos y si ya está bajando
    private List<Ola> olasActivas = new List<Ola>();
    [HideInInspector] public float multiplicadorVelocidad = 1f;

    private string[] opsFacil   = { "+", "-", "+", "-", "+", "-" };
    private string[] opsMedio   = { "+", "-", "+", "-", "*", "*", "*" };
    private string[] opsDificil = { "+", "-", "+", "-", "*", "*", "*", "/", "/", "/" };

    // Clase interna que representa una ola (casillas + puerta)
    private class Ola
    {
        public List<Transform> objetos = new List<Transform>();
        public bool bajando = false;
    }

    void Start()
    {
        carriles = new float[] { carrilIzquierda, carrilCentro, carrilDerecha };
        SpawnOla();
    }

    void Update()
    {
        float velocidad = (GameManager.Instance != null
                        ? GameManager.Instance.VelocidadActual()
                        : 3f) * multiplicadorVelocidad;

        // Mover solo las olas que ya están bajando
        foreach (var ola in olasActivas)
        {
            if (!ola.bajando) continue;
            foreach (var obj in ola.objetos)
            {
                if (obj != null)
                    obj.position += Vector3.down * velocidad * Time.deltaTime;
            }
        }

        // Limpiar objetos fuera de pantalla y olas vacías
        foreach (var ola in olasActivas)
        {
            ola.objetos.RemoveAll(t => {
                if (t == null) return true;
                if (t.position.y < destroyY)
                {
                    Destroy(t.gameObject);
                    return true;
                }
                return false;
            });
        }
        olasActivas.RemoveAll(o => o.objetos.Count == 0);
    }

    void SpawnOla()
    {
        StartCoroutine(SpawnOlaCoroutine());
    }

    IEnumerator SpawnOlaCoroutine()
    {
        int rango    = GameManager.Instance != null ? GameManager.Instance.RangoOperacion : 5;
        string[] ops = ObtenerOperadores();

        int valorBase = GameManager.Instance != null ? GameManager.Instance.ValorJugador : 0;
        if (GameManager.Instance != null)
            GameManager.Instance.ValorBase = valorBase;

        // Generar meta diferente al valorBase
        int meta;
        int intentosMeta = 0;
        int metaMin = GameManager.Instance.Dificultad == "dificil" ? -rango : 1;
        do { meta = Random.Range(metaMin, rango + 1); intentosMeta++; }
        while (meta == valorBase && intentosMeta < 50);
        if (meta == valorBase) meta = valorBase + 1;

        int diferencia     = meta - valorBase;
        int carrilSolucion = Random.Range(0, 3);

        string[] opsCarril  = new string[3];
        int[]    numsCarril = new int[3];
        int[]    resultados = new int[3];

        // Generar casillas
        for (int i = 0; i < 3; i++)
        {
            if (i == carrilSolucion)
            {
                string op = "+";
                int num   = 0;
                int intentos = 0;

                do {
                    op = ops[Random.Range(0, ops.Length)];
                    switch (op)
                    {
                        case "+": num = meta - valorBase; break;
                        case "-": num = valorBase - meta; break;
                        case "*":
                            num = (valorBase != 0 && meta % valorBase == 0) ? meta / valorBase : -1;
                            break;
                        case "/":
                            num = (meta != 0 && valorBase % meta == 0) ? valorBase / meta : -1;
                            break;
                    }
                    intentos++;
                } while (num <= 0 && intentos < 20);

                if (num <= 0)
                {
                    op  = System.Array.Exists(ops, o => o == "+") ? "+" : "-";
                    num = Mathf.Abs(meta - valorBase);
                }

                opsCarril[i]  = op;
                numsCarril[i] = num;
                resultados[i] = meta;
            }
            else
            {
                string opDist; int numDist; int resultadoDist; int intentos = 0;
                do {
                    opDist  = ops[Random.Range(0, ops.Length)];
                    numDist = Random.Range(1, rango + 1);
                    resultadoDist = opDist switch {
                        "+" => valorBase + numDist,
                        "-" => valorBase - numDist,
                        "*" => valorBase * numDist,
                        "/" => numDist != 0 ? valorBase / numDist : valorBase + 1,
                        _   => valorBase + 1
                    };
                    intentos++;
                } while (resultadoDist == meta && intentos < 20);

                opsCarril[i]  = opDist;
                numsCarril[i] = numDist;
                resultados[i] = resultadoDist;
            }
        }

        Shuffle(ref opsCarril, ref numsCarril, ref resultados);

        // Crear la ola
        Ola ola = new Ola();

        // Instanciar casillas — fijas en spawnY
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
            ola.objetos.Add(go.transform);
        }

        olasActivas.Add(ola);

        // Esperar tiempoEspera — las casillas se ven fijas arriba
        yield return new WaitForSeconds(tiempoEspera);

        // Instanciar puerta justo encima de las casillas
        GameObject puertaGO = Instantiate(prefabPuerta, transform);
        puertaGO.transform.position = new Vector3(carrilCentro, spawnY + distanciaPuerta, 0f);

        PuertaController puerta = puertaGO.GetComponent<PuertaController>();
        if (puerta != null) puerta.Configurar(meta);

        ola.objetos.Add(puertaGO.transform);

        // Activar el movimiento hacia abajo
        ola.bajando = true;
    }

    public void ForzarNuevaOla()
    {
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
            string tOp = ops[i]; ops[i] = ops[j]; ops[j] = tOp;
            int tNum = nums[i]; nums[i] = nums[j]; nums[j] = tNum;
            int tRes = res[i];  res[i]  = res[j];  res[j]  = tRes;
        }
    }
}