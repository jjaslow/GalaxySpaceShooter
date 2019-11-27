using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    [Range(1, 10)]
    public int speed;
    [SerializeField]
    private readonly float fireRate = 0.15f;
    private float nextFire = 0f;

    public GameObject laserPrefab;
    public GameObject tripleShotPrefab;
    public GameObject shieldsImage;

    private bool _tripleShotEnabled = false;
    private bool _shieldsUp = false;
    private bool _autoFireOn = false;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score=0;

    [SerializeField]
    private GameObject Canvas;
    private UIManager _uiManager;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionClip;
    [SerializeField]
    private AudioClip _powerUpClip;

    [SerializeField]
    private GameObject _fireLeft;
    [SerializeField]
    private GameObject _fireRight;
    [SerializeField]
    private GameObject _fireMiddle;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
        _uiManager = Canvas.GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _fireLeft.SetActive(false);
        _fireRight.SetActive(false);
        _fireMiddle.SetActive(false);
        shieldsImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        Wrap();

        if ((Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire) || (_autoFireOn && _lives>=0))
            Shoot();     
            
    }

    private void MovePlayer()
    {
        //transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        //transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        transform.Translate(new Vector3
            (Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            0
            ) * speed * Time.deltaTime);
    }

    private void Wrap()
    {
        if (transform.position.x > 11.25)
            transform.position = new Vector3(-11.25f, transform.position.y, transform.position.z);
        else if (transform.position.x < -11.25)
            transform.position = new Vector3(11.25f, transform.position.y, transform.position.z);

        if (transform.position.y >= 5.9)
            transform.position = new Vector3(transform.position.x, 5.9f, transform.position.z);
        else if (transform.position.y <= -3.5)
            transform.position = new Vector3(transform.position.x, -3.5f, transform.position.z);

    }

    private void Shoot()
    {

        if(!_tripleShotEnabled)
        {
            Instantiate(laserPrefab, transform.position + (.95f * Vector3.up), Quaternion.identity);
        }
        else
        {
            Instantiate(tripleShotPrefab, transform.position, Quaternion.identity);
        }

        nextFire = Time.time + fireRate;

    }

    public void Damage()
    {
        if(!_shieldsUp)
        {
            _lives--;

            _uiManager.UpdateLivesImage(_lives);

            _audioSource.clip = _explosionClip;
            _audioSource.volume = 0.096f;
            _audioSource.Play();
            //transform.position = Vector3.zero;

            //GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
            //foreach (GameObject e in enemies)
            //    Destroy(e);

            //GameObject[] powerUps = GameObject.FindGameObjectsWithTag("powerUp");
            //foreach (GameObject pu in powerUps)
            //    Destroy(pu);
            FireDamage();
        }
        else
        {
            shieldsImage.SetActive(false);
            _shieldsUp = false;
        }

        if (_lives < 0)
        {
            GameObject.Find("Spawn_Manager").SetActive(false);
            Destroy(GameObject.Find("EnemyParent"));
            _fireLeft.SetActive(false);
            _fireRight.SetActive(false);
            _fireMiddle.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);


            gameObject.GetComponent<Renderer>().enabled = false;

            GameObject[] powerUps = GameObject.FindGameObjectsWithTag("powerUp");
            foreach (GameObject pu in powerUps)
                Destroy(pu);

            _uiManager.GameOver();
            GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>().GameOver();
        }
            
    }

    public void PlayPowerUpClip()
    {
        _audioSource.clip = _powerUpClip;
        _audioSource.volume = 1f;
        _audioSource.Play();
    }

    public void TripleShotPowerUp()
    {
        _tripleShotEnabled = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _tripleShotEnabled = false;

    }


    public void SpeedPowerUp()
    {
        speed*=2;
        StartCoroutine(SpeedPowerDown());
    }

    IEnumerator SpeedPowerDown()
    {
        yield return new WaitForSeconds(5f);
        speed /= 2;

    }


    public void AutoFirePowerUp()
    {
        StartCoroutine(AutoFireToggle());
        StartCoroutine(AutoFirePowerDown());
    }

    IEnumerator AutoFireToggle()
    {
        float counter = Time.time + 5f;

        while(Time.time < counter)
        {
            _autoFireOn = true;
            yield return new WaitForSeconds(.05f);
            _autoFireOn = false;
            yield return new WaitForSeconds(.05f);
        }

    }

    IEnumerator AutoFirePowerDown()
    {
        yield return new WaitForSeconds(5f);
        _autoFireOn = false;

    }


    public void ShieldPowerUp()
    {
        if(!_shieldsUp)
        {
            _shieldsUp = true;
            shieldsImage.SetActive(true);
        }
        
    }

    public void AddScore(int value)
    {
        _score += value;
        _uiManager.UpdateScore(_score);
    }

    private void FireDamage()
    {

        switch (_lives)
        {
            case 2:
                _fireLeft.SetActive(true);
                break;
            case 1:
                _fireRight.SetActive(true);
                break;
            case 0:
                _fireMiddle.SetActive(true);
                break;

        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "laserEnemy")
        {
            Destroy(other.gameObject);
            Damage();

        }

    }


}
