using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEnemy : MonoBehaviour
{
    private float _speed = 0.75f;
    private Player Player;

    [SerializeField]
    private GameObject _ExplosionPrefab;

    private AudioSource _audioSource;
    bool _isAlive;


    private void Start()
    {
        _isAlive = true;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        Transform _playerLocation = Player.transform;
        Vector3 vectorToTarget = transform.position - _playerLocation.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        transform.Translate(-1 * _speed * Time.deltaTime, 0, 0);

    }






    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "laser")
        {
            _isAlive = false;
            _audioSource.Play();
            Destroy(other.gameObject);
            GameObject explosion = Instantiate(_ExplosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2.5f);
            Player.AddScore(10);
            StartCoroutine(DestroyEnemy());
        }
        else if (other.gameObject.tag == "Player")
        {
            _isAlive = false;
            other.GetComponent<Player>().Damage();
            GameObject explosion = Instantiate(_ExplosionPrefab, transform.position, transform.rotation);
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

    private void OnDestroy()
    {
        GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().HomingEnemyExists = false;
    }

}
