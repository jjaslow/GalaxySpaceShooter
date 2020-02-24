﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject EnemyPrefab;
    [SerializeField]
    private GameObject HomingEnemyPrefab;
    [SerializeField]
    private Transform EnemySpawnParent;
    [SerializeField]
    private GameObject BossEnemy;
    public bool HomingEnemyExists = false;

    [SerializeField]
    private GameObject[] PowerUpPrefabs;
    [SerializeField]
    private GameObject ReloadAmmoPrefab;


    [SerializeField]
    int _waveSystemFactor = 0;
    float _nextWaveTime = 0;
    private float _timeTillNextWave = 30f;
    Coroutine enemySpawnCoroutine;
    Coroutine enemyHomingSpawnCoroutine;

    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        WaveText(++_waveSystemFactor);
        _nextWaveTime = _timeTillNextWave;
        enemySpawnCoroutine = StartCoroutine(SpawnEnemies());
        enemyHomingSpawnCoroutine = StartCoroutine(SpawnHomingEnemies());
        StartCoroutine(SpawnPowerUps());
        StartCoroutine(SpawnReloadAmmo());
    }

    private void Update()
    {
        if (_nextWaveTime < 0)
        {
            _nextWaveTime = _timeTillNextWave;
            WaveText(++_waveSystemFactor);
        }
        _nextWaveTime -= Time.deltaTime;
    }

    private void WaveText(int x)
    {
        //Debug.Log("Wave " + x);
        if(x==4) //boss wave
        {
            x = -1;
            StopCoroutine(enemySpawnCoroutine);
            StopCoroutine(enemyHomingSpawnCoroutine);
            SpawnBossEnemy();
            foreach (Transform eachChild in EnemySpawnParent.transform)
                Destroy(eachChild.gameObject);
        }
        _uiManager.SetWaveText(x);
    }


    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(3f);
        
        while (true)
        {
            Vector3 EnemyPosition;
            Quaternion EnemyRotation = Quaternion.identity;
            bool _enemyTravelTop2Bottom = true;
            int _enemyMovement = Random.Range(0, 10);

            if (_enemyMovement < 2 ) //2  adjust position and direction for 20%;   if 20% then dont shoot
            {
                float xPos = _enemyMovement == 0 ? -11 : 11;   //spawn on left or right side
                float yPos = Random.Range(-4, 6);  //-4 to 6
                EnemyPosition = new Vector3(xPos, yPos, 0);

                Transform _playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
                Vector3 vectorToTarget = EnemyPosition - _playerLocation.position;
                float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg)-90;
                EnemyRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                _enemyTravelTop2Bottom = false;
            }
            else  //start at top and go straight down as normal
            {
                float xPos = Random.Range(-9f, 9f);
                EnemyPosition = new Vector3(xPos, 7.25f, 0);
            }

            Enemy e = Instantiate(EnemyPrefab, EnemyPosition, EnemyRotation, EnemySpawnParent).GetComponent<Enemy>();
            e.PowerUp(_enemyTravelTop2Bottom);

            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f)/_waveSystemFactor);
        }
    }

    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(10f);
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(7, 15));

            Vector3 PowerUpPosition = new Vector3(Random.Range(-9f, 9f), 7.25f, 0);

            Instantiate(PowerUpPrefabs[Random.Range(0,PowerUpPrefabs.Length)], PowerUpPosition, Quaternion.identity);


        }
        
    }


    IEnumerator SpawnReloadAmmo()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10, 15));

            Vector3 PowerUpPosition = new Vector3(Random.Range(-9f, 9f), 7.25f, 0);

            Instantiate(ReloadAmmoPrefab, PowerUpPosition, Quaternion.identity);


        }

    }

    IEnumerator SpawnHomingEnemies()
    {
       
        yield return new WaitForSeconds(25f);
        
        while (true)
        {
            if(!HomingEnemyExists)
            {
                HomingEnemyExists = true;

                int xPos;
                if (Random.value < 0.5f)
                    xPos  = -11;
                else
                    xPos = 11;

                Vector3 Position = new Vector3(xPos, Random.Range(-4, 6), 0);
                Instantiate(HomingEnemyPrefab, Position, Quaternion.identity, EnemySpawnParent);
            }
            
            yield return new WaitForSeconds(Random.Range(25, 35));


        }
    }

    void SpawnBossEnemy()
    {
        BossEnemy.SetActive(true);
    }

}
