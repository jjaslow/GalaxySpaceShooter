using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private GameObject _ExplosionPrefab;
    private SpawnManager _SpawnManager;
    private UIManager _uiManager;

    private AudioSource _audioSource;

    private void Start()
    {
        _SpawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _SpawnManager.enabled =false;
        _audioSource = GetComponent<AudioSource>();
        _uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        _uiManager.noAmmoText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 20f) * Time.deltaTime);
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "laser")
        {
            _audioSource.Play();
            _SpawnManager.enabled = true;
            Destroy(other.gameObject);
            GameObject explosion = Instantiate(_ExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
            StartCoroutine(DestroyAsteroid());
            GetComponent<CircleCollider2D>().enabled = false;
        }
        


    }

    IEnumerator DestroyAsteroid()
    {
        yield return new WaitForSeconds(0.15f);
        GetComponent<Renderer>().enabled = false;
        _uiManager.noAmmoText.SetActive(true);
        Destroy(gameObject, 3f);
    }

}
