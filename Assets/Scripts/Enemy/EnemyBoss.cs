using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    GameObject _target;
    [SerializeField]
    GameObject _particles;
    [SerializeField]
    GameObject _focusParticle;
    [SerializeField]
    particleAttractorLinear PAL;  //linear (eg Player)
    [SerializeField]
    particleAttractorSelf PAS;  //self
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    MeshRenderer _mesh;
    AudioSource _audioSource;

    bool _isAlive = false;
    [SerializeField]
    bool _hasFired = false;
    [SerializeField]
    bool _hasCharged = false;
    GameObject _player;
    int _travelSpeed = 5;
    int _lives = 100;


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _particles.SetActive(false);
        PAL.enabled = false;
        PAS.enabled = false;
        _player = GameObject.FindGameObjectWithTag("Player");
        _mesh.sortingLayerName = "Foreground";
        _mesh.sortingOrder = 1;
        Debug.DrawRay(_player.transform.position, Vector3.right * 1.5f, Color.red);
        Debug.DrawLine(_player.transform.position, _player.transform.position + (Vector3.right * 1.5f), Color.red);
    }

    private void OnEnable()
    {
        _isAlive = true;
    }


    private void Update()
    {
        if(_isAlive && transform.position.x<0)
        {
            transform.Translate(Vector3.right * Time.deltaTime * _travelSpeed, Space.World);
        }
        if (transform.position.x > -.25 && !_hasCharged)
        {
            _hasCharged = true;
            ChargeWeapon();
        }
        if(_hasFired)
        {
            transform.Translate(Vector3.right * Time.deltaTime * _travelSpeed, Space.World);
        }
        if(transform.position.x > 14)
        {
            transform.position = new Vector3(-14.25f, 4, 0);
            _hasFired = false;
            _hasCharged = false;
        }


    }


    public void ChargeWeapon()
    {
        _particles.SetActive(true);
        PAS.enabled = true;
        StartCoroutine(FireWeapon());
    }

    IEnumerator FireWeapon()
    {
        yield return new WaitForSeconds(Random.Range(1f, 4));

        //set target location
        _target.transform.position = _player.transform.position;
        _focusParticle.transform.position = _target.transform.position;
        _focusParticle.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.25f, .5f));

        //fire
        PAS.enabled = false;
        PAL.enabled = true;
        if (Vector3.Distance(_target.transform.position, _player.transform.position) < 1.875)
            _player.GetComponent<Player>().Damage();
        yield return new WaitForSeconds(2f);

        //reset
        _target.transform.position = transform.position;
        _particles.SetActive(false);
        _focusParticle.SetActive(false);
        PAL.enabled = false;
        _hasFired = true;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "laser" || other.gameObject.tag == "Player")
        {
            _audioSource.Play();
            Vector2 _pos = new Vector2(_player.transform.position.x, _player.transform.position.y);
            Instantiate(explosion, other.ClosestPoint(_pos), Quaternion.identity);
            _lives--;
            if (_lives < 1 && _isAlive)
                DestroyEnemy();
        }


        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().Damage();
        }

        
    }

    void DestroyEnemy()
    {
        _isAlive = false;
        GameObject finalExplosion = Instantiate(explosion, transform.position, transform.rotation);
        finalExplosion.transform.localScale = new Vector3(2, 2, 2);
        Destroy(finalExplosion, 2.5f);
        Destroy(gameObject, .5f);
        Debug.Log("you win!!!!!");
    }

}

