using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    public int speed;
    [SerializeField]
    private readonly float fireRate = 0.15f;
    private float nextFire = 0f;

    public GameObject laserPrefab;
    public GameObject tripleShotPrefab;
    public GameObject shieldsImage;

    private bool _tripleShotEnabled = false;
    private bool _shieldsUp = false;
    [SerializeField]
    private int _shieldStrength = 0;
    private bool _autoFireOn = false;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score=0;
    [SerializeField]
    private int _ammoCount;
    [SerializeField]
    private float _thrustPower = 5;
    [SerializeField]
    private bool _thrustActive = false;
    [SerializeField]
    private bool _canStartThrust = true;

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

    private Camera mainCamera;
    private bool _shakeCamera = false;

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
        _ammoCount = 16;   //16
        mainCamera = Camera.main;
        _uiManager.ThrustBar(_thrustPower);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        Wrap();

        if ((Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire && _ammoCount>0 && _lives >= 0) || (_autoFireOn && _lives>=0))
            Shoot();     

        if(_shakeCamera && _lives >= 0)
        {
            float x = Random.Range(-.25f, .25f);
            float y = Random.Range(-.25f, .25f);
            mainCamera.transform.localEulerAngles = new Vector3(x, y, 0);
        }

        if(!_thrustActive && _thrustPower<10)
        {
            _thrustPower += .01f;
            _uiManager.ThrustBar(_thrustPower);

            if (_thrustPower >= 5)
                _canStartThrust = true;
        }
        else if (_thrustActive)
        {
            _thrustPower -= .01f;
            _uiManager.ThrustBar(_thrustPower);
        }

            
    }

    private void MovePlayer()
    {
        //transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        //transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.LeftShift) &&  _thrustPower>5 && _canStartThrust)
        {
            speed *= 2;
            _thrustActive = true;
            _canStartThrust = false;
        }
            
        if ((Input.GetKeyUp(KeyCode.LeftShift) && _thrustActive) || (_thrustPower<.25f && _thrustActive))
        {
            speed /= 2;
            _thrustActive = false;
            if(_thrustPower<5)
                _canStartThrust = false;
        }
            

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
        if (!_autoFireOn)
            _ammoCount--;

        _uiManager.AmmoCountDisplay(_ammoCount, _autoFireOn);
        nextFire = Time.time + fireRate;

    }

    public void Damage()
    {
        if(_shieldStrength == 0)   //(!_shieldsUp)
        {

            _lives--;
            StartCoroutine(ShakeCamera());

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
        else if (_shieldStrength>1)
        {
            _shieldStrength--;

            float shieldOpacity = shieldsImage.GetComponent<SpriteRenderer>().color.a;
            shieldOpacity -= .33f;
            shieldsImage.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, shieldOpacity);
        }
        else
        {
            shieldsImage.SetActive(false);
            _shieldsUp = false;
            _shieldStrength--;
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

    public void ReloadPowerUp()
    {
        _ammoCount = 15;
        _uiManager.AmmoCountDisplay(_ammoCount, _autoFireOn);

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
        _uiManager.AmmoCountDisplay(_ammoCount, _autoFireOn);
    }


    public void ShieldPowerUp()
    {
        //if(!_shieldsUp)
        {
            _shieldsUp = true;
            shieldsImage.SetActive(true);
            _shieldStrength = 3;
            shieldsImage.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        
    }

    public void AddLifePowerUp()
    {
        if(_lives<3)
        {
            _uiManager.UpdateLivesImage(++_lives);
            FireDamage();
        }
        
    }

    public void BadPowerUp()
    {
        transform.position = new Vector3(Random.Range(9.1f, 9.1f), Random.Range(-3f, 5.5f), 0);
        _ammoCount = 0;
        _uiManager.AmmoCountDisplay(_ammoCount, _autoFireOn);
    }

    public void AddScore(int value)
    {
        _score += value;
        _uiManager.UpdateScore(_score);
    }

    IEnumerator ShakeCamera()
    {
        _shakeCamera = true;
        yield return new WaitForSeconds(0.75f);
        _shakeCamera = false;
        mainCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void FireDamage()
    {

        switch (_lives)
        {
            case 3:
                _fireLeft.SetActive(false);
                _fireRight.SetActive(false);
                _fireMiddle.SetActive(false);
                break;
            case 2:
                _fireLeft.SetActive(true);
                _fireRight.SetActive(false);
                _fireMiddle.SetActive(false);
                break;
            case 1:
                _fireLeft.SetActive(true);
                _fireRight.SetActive(true);
                _fireMiddle.SetActive(false);
                break;
            case 0:
                _fireLeft.SetActive(true);
                _fireRight.SetActive(true);
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
