using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject boidPrefab;
    public int cantidad = 20;
    public float distancia = 10f;

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * distancia;
            randomPosition.y = 0;
            Instantiate(boidPrefab, randomPosition, Quaternion.identity);
        }
    }
}
