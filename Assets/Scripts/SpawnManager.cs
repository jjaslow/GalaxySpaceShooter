using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject EnemyPrefab;
    [SerializeField]
    private Transform EnemySpawnParent;

    [SerializeField]
    private GameObject[] PowerUpPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerUps());

    }


    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            float xPos = Random.Range(-9f, 9f);
            Vector3 EnemyPosition = new Vector3(xPos, 7.25f, 0);

            Instantiate(EnemyPrefab, EnemyPosition, Quaternion.identity, EnemySpawnParent);
            yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
        }
    }

    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(3f);
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(7, 15));

            Vector3 PowerUpPosition = new Vector3(Random.Range(-9f, 9f), 7.25f, 0);

            Instantiate(PowerUpPrefabs[Random.Range(0,PowerUpPrefabs.Length)], PowerUpPosition, Quaternion.identity);


        }
        
    }


}
