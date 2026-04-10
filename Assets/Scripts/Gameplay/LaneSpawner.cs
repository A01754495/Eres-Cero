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

    [Header("Distancia vertical entre olas")]
    public float distanciaEntreOlas = 8.6f;
    public float distanciaPuerta    = 2.7f;

    private float[] carriles;
    private float   timerProximaOla; // timer en segundos hasta siguiente ola
    private bool    primeraOla = true;

    private string[] opsFacil   = { "+", "-" };
    private string[] opsMedio   = { "+", "-", "*" };
    private string[] opsDificil = { "+", "-", "*", "/" };

    void Start()
    {
        carriles = new float[] { carrilIzquierda, carrilCentro, carrilDerecha };
        SpawnOla(); // primera ola inmediata
        // Calcular cuánto tiempo tarda la primera ola en bajar hasta dejar espacio
        float vel = GameManager.Instance != null ? GameManager.Instance.VelocidadActual() : 3f;
        timerProximaOla = distanciaEntreOlas / vel;
        primeraOla = false;
    }

    void Update()
    {
        float velocidad = GameManager.Instance != null
                        ? GameManager.Instance.VelocidadActual()
                        : 3f;

        // Mover todos los hijos hacia abajo
        foreach (Transform hijo in transform)
            hijo.position += Vector3.down * velocidad * Time.deltaTime;

        // Destruir objetos fuera de pantalla
        List<Transform> aEliminar = new List<Transform>();
        foreach (Transform hijo in transform)
            if (hijo.position.y < destroyY)
                aEliminar.Add(hijo);
        foreach (var h in aEliminar)
            Destroy(h.gameObject);

        // Countdown para siguiente ola
        timerProximaOla -= Time.deltaTime;
        if (timerProximaOla <= 0f)
        {
            SpawnOla();
            // Recalcular timer con velocidad actual (va aumentando)
            velocidad = GameManager.Instance != null
                      ? GameManager.Instance.VelocidadActual()
                      : 3f;
            timerProximaOla = distanciaEntreOlas / velocidad;
        }
    }

    void SpawnOla()
    {
        int rango    = GameManager.Instance != null ? GameManager.Instance.RangoOperacion : 5;
        string[] ops = ObtenerOperadores();

        int    carrilSolucion = Random.Range(0, 3);
        int    meta           = Random.Range(1, rango + 1);

        string[] opsCarril  = new string[3];
        int[]    numsCarril = new int[3];

        for (int i = 0; i < 3; i++)
        {
            if (i == carrilSolucion)
            {
                opsCarril[i]  = "+";
                numsCarril[i] = meta;
            }
            else
            {
                opsCarril[i]  = ops[Random.Range(0, ops.Length)];
                numsCarril[i] = Random.Range(1, rango + 1);
            }
        }

        Shuffle(ref opsCarril, ref numsCarril);

        // Las 3 casillas a la misma Y (spawnY)
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(prefabCasilla, transform);
            go.transform.position = new Vector3(carriles[i], spawnY, 0f);
            go.GetComponent<CasillaOperacion>()?.Configurar(opsCarril[i], numsCarril[i]);
        }

        // Puerta justo encima de las casillas
        GameObject puertaGO = Instantiate(prefabPuerta, transform);
        puertaGO.transform.position = new Vector3(carrilCentro, spawnY + distanciaPuerta, 0f);
        puertaGO.GetComponent<PuertaController>()?.Configurar(meta);
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

    void Shuffle(ref string[] ops, ref int[] nums)
    {
        for (int i = ops.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ops[i], ops[j])   = (ops[j], ops[i]);
            (nums[i], nums[j]) = (nums[j], nums[i]);
        }
    }
}