using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Autos disponibles")]
    public CarData[] autos;

    [Header("Recorrido")]
    public Transform puntoSpawn;
    public Transform puntoFinal;

    [Header("Spawn")]
    public float tiempoEntreAutos = 4f;

    private ISimpleDictionary<string, CarData>
        autosDictionary;

    private string[] idsAutos;

    private void Awake()
    {
        //implementación ESTÁTICA.
        autosDictionary =
            new SimpleArrayDictionary<string, CarData>(
                autos.Length
            );

        // Cargamos los datos
        for (int i = 0; i < autos.Length; i++)
        {
            autosDictionary.Add(
                autos[i].id,
                autos[i]
            );
        }

        idsAutos = autosDictionary.Keys();
    }

    private void Start()
    {
        StartCoroutine(SpawnAutomatico());
    }

    private IEnumerator SpawnAutomatico()
    {
        while (true)
        {
            CrearAutoAleatorio();

            yield return new WaitForSeconds(
                tiempoEntreAutos
            );
        }
    }

    private void CrearAutoAleatorio()
    {
        if (autosDictionary.IsEmpty)
            return;

        int indice =
            Random.Range(0, idsAutos.Length);

        string idElegido =
            idsAutos[indice];

        CarData datosAuto;

        if (autosDictionary.TryGetValue(
            idElegido,
            out datosAuto))
        {
            GameObject nuevoAuto =
                Instantiate(
                    datosAuto.prefab,
                    puntoSpawn.position,
                    puntoSpawn.rotation
                );

            CarMovement movimiento =
                nuevoAuto.GetComponent<CarMovement>();

            if (movimiento != null)
            {
                movimiento.Inicializar(
                    puntoFinal,
                    datosAuto.velocidad
                );
            }
        }
    }
}