using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 4.0f;
    private Player Player;

    [SerializeField]
    private GameObject _ExplodingEnemyPrefab;
    [SerializeField]
    private GameObject laserPrefab;

    private AudioSource _audioSource;
    bool _isAlive;
    bool _enemyCanShoot = true;
    Coroutine _coRoutine;


    private void Start()
    {
        _isAlive = true;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if(Random.Range(0,10)==0) //1 of 10 will shoot
        {
            _coRoutine = StartCoroutine(Shoot());
        }
            
    }

    public void Init(bool enemyCanShoot)
    {
        _enemyCanShoot = enemyCanShoot;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -1 * _speed * Time.deltaTime, 0);

        if (transform.position.y < -5.5 || Mathf.Abs(transform.position.y)>12)
        {
            //commented code returns enemy to top instead of destroying.
            //float xPos = Random.Range(-9f, 9f);
            //transform.position = new Vector3(xPos, 7.25f, transform.position.z);
            Destroy(gameObject);
        }
            
        
    }



    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(Random.Range(1, 2));

        while (true && _isAlive && _enemyCanShoot)
        {
            Instantiate(laserPrefab, transform.position + (.95f * Vector3.down), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1, 2));
        }
        
    

    }



    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "laser")
        {
            _isAlive = false;
            if(_coRoutine != null)
                StopCoroutine(_coRoutine);
            _audioSource.Play();
            Destroy(other.gameObject);
            GameObject explosion = Instantiate(_ExplodingEnemyPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2.5f);
            Player.AddScore(10);
            StartCoroutine(DestroyEnemy());
        }
        else if (other.gameObject.tag == "Player")
        {
            _isAlive = false;
            if (_coRoutine != null)
                StopCoroutine(_coRoutine);
            other.GetComponent<Player>().Damage();
            GameObject explosion = Instantiate(_ExplodingEnemyPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2.5f);
            StartCoroutine(DestroyEnemy());
        }
    }


    IEnumerator DestroyEnemy()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, 3f);
    }



}
