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
    [SerializeField]
    private GameObject laserUpPrefab;

    private AudioSource _audioSource;
    bool _isAlive;
    bool _enemyCanShoot = true;
    Coroutine _shootCoRoutine;

    GameObject shieldsImage;
    bool _shieldsUp = false;
    bool _isAggressive = false;


    private void Awake()
    {
        _isAlive = true;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        shieldsImage = transform.GetChild(0).gameObject;
    }

    public void PowerUp(bool _enemyTravelTop2Bottom)
    {
        _enemyCanShoot = _enemyTravelTop2Bottom;

        if(_enemyTravelTop2Bottom)
        {
            int _specialPowerEnemy = Random.Range(0, 10);
            if (_specialPowerEnemy == 0) //1 of 10 will shoot
            {
                _shootCoRoutine = StartCoroutine(Shoot(false));
                //Debug.Log("SPECIAL POWER: Shoot");
            }
            else if (_specialPowerEnemy == 1) //1 of 10 can shoot backwards
            {
                _shootCoRoutine = StartCoroutine(Shoot(true));
                //Debug.Log("SPECIAL POWER: Shoot back");
            }
            else if (_specialPowerEnemy == 2) //1 of 10 will have shields
            {
                _shieldsUp = true;
                shieldsImage.SetActive(true);
                //Debug.Log("SPECIAL POWER: shield");
            }
            else if (_specialPowerEnemy == 3) //1 of 10 will be aggressive
            {
                _isAggressive = true;
                transform.GetChild(1).gameObject.SetActive(true);
                //Debug.Log("SPECIAL POWER: aggressive");
            }
            else if (_specialPowerEnemy == 4) //1 of 10 can dodge laser
            {
                transform.GetChild(2).gameObject.SetActive(true);
                transform.GetChild(3).gameObject.SetActive(true);
                //Debug.Log("SPECIAL POWER: dodge");
            }
            //else
            //    Debug.Log("SPECIAL POWER: NONE");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, -1 * _speed * Time.deltaTime, 0);

        if(_isAggressive && 
           Mathf.Abs((transform.position - Player.transform.position).magnitude)<4  )
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Time.deltaTime*5);
        }

        if (transform.position.y < -5.5 || Mathf.Abs(transform.position.y)>12)
        {
            //commented code returns enemy to top instead of destroying.
            //float xPos = Random.Range(-9f, 9f);
            //transform.position = new Vector3(xPos, 7.25f, transform.position.z);
            Destroy(gameObject);
        }
            
        
    }



    IEnumerator Shoot(bool canShootbackwards)
    {
        yield return new WaitForSeconds(Random.Range(1, 2));

        while (true && _isAlive && _enemyCanShoot)
        {
            bool isEnemyHigherUp = transform.position.y > Player.transform.position.y;
            if(canShootbackwards && !isEnemyHigherUp)
                Instantiate(laserUpPrefab, transform.position - (1.25f * Vector3.down), Quaternion.identity);
            else
                Instantiate(laserPrefab, transform.position + (.95f * Vector3.down), Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(1, 2));
        }
        
    

    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "laser" || other.gameObject.tag == "Player")
        {
            if (!_shieldsUp)
                DestroyEnemy();
        }
        

        if (other.gameObject.tag == "laser")
        {
            Destroy(other.gameObject);
            if (_shieldsUp)
            {
                shieldsImage.SetActive(false);
                _shieldsUp = false;
            }
            else
            {
                _audioSource.Play();
                Player.AddScore(10);
            }
        }
        else if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().Damage();
            shieldsImage.SetActive(false);
            _shieldsUp = false;
        }
    }

    void DestroyEnemy()
    {
        _isAlive = false;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        if (_shootCoRoutine != null)
            StopCoroutine(_shootCoRoutine);
        GameObject explosion = Instantiate(_ExplodingEnemyPrefab, transform.position, transform.rotation);
        Destroy(explosion, 2.5f);
        StartCoroutine(ClearEnemy());
    }


    IEnumerator ClearEnemy()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, 3f);
    }





}
