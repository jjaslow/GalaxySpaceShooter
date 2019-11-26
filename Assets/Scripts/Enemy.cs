using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 4.0f;
    private Player Player;

    [SerializeField]
    private GameObject _ExplodingEnemyPrefab;

    private AudioSource _audioSource;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -1 * _speed * Time.deltaTime, 0);

        if (transform.position.y < -5.5)
        {
            float xPos = Random.Range(-9f, 9f);
            transform.position = new Vector3(xPos, 7.25f, transform.position.z);
        }
            
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "laser")
        {
            _audioSource.Play();
            Destroy(other.gameObject);
            GameObject explosion = Instantiate(_ExplodingEnemyPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2.5f);
            Player.AddScore(10);
            StartCoroutine(DestroyEnemy());
        }
        else if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().Damage();
            GameObject explosion = Instantiate(_ExplodingEnemyPrefab, transform.position, Quaternion.identity);
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
