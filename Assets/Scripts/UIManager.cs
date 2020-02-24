using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    public GameObject noAmmoText;
    private Text noAmmoTextCopy;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private GameObject RestartText;
    [SerializeField]
    private GameObject WaveText;

    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _livesImagePrefab;

    [SerializeField]
    private Image thrustBar;

    private bool _noAmmo = false;


    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: 0";
        GameOverText.SetActive(false);
        //noAmmoText.SetActive(false);
        RestartText.SetActive(false);
        noAmmoTextCopy = noAmmoText.GetComponent<Text>();
        noAmmoTextCopy.color = Color.white;
        noAmmoTextCopy.text = "Ammo Left: 25 / 25";
    }


    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void AmmoCountDisplay(int ammoCount, bool autoFireOn)
    {
        //_noAmmo = false;

        if (autoFireOn)
        {
            noAmmoTextCopy.color = Color.green;
            noAmmoTextCopy.text = "Unlimited Autofire!";
        }
        else if(ammoCount == 0)
        {
            //_noAmmo = true;
            noAmmoTextCopy.color = Color.red;
            noAmmoTextCopy.text = "OUT OF AMMO!";
            //StartCoroutine(FlickerNoAmmoText());
        }
        else
        {
            noAmmoTextCopy.text = "Ammo Left: " + ammoCount + " / 25";
            noAmmoTextCopy.color = Color.white;
        }
            
    }

    public void AmmoText(bool outOfAmmo)
    {
        if(outOfAmmo)
        {
            noAmmoText.SetActive(true);
            _noAmmo = true;
            StartCoroutine(FlickerNoAmmoText());
        }
        else
        {
            noAmmoText.SetActive(false);
            _noAmmo = false;
            StopCoroutine(FlickerNoAmmoText());
            noAmmoText.SetActive(false);
        }
            
    } //depreciated

    public void UpdateLivesImage(int lives)
    {
        if(lives >=0)
        {
            _livesImage.sprite = _livesImagePrefab[lives];
        }
        
    }

    public void ThrustBar(float value)
    {
        thrustBar.fillAmount = value / 10;

        if (value < 5)
            thrustBar.color = new Color(1, 0, 44 / 255);
        else
            thrustBar.color = new Color(0, 1, 44 / 255);
    }

    public void GameOver(bool _didIWin)
    {
        noAmmoText.SetActive(false);
        _noAmmo = false;
        StopCoroutine(FlickerNoAmmoText());

        if (_didIWin)
            GameOverText.GetComponent<Text>().text = "YOU WIN!!!";
        GameOverText.SetActive(true);
        RestartText.SetActive(true);
        StartCoroutine(FlickerGameOverText());
    }

    public void SetWaveText(int wave)
    {
        if(wave==-1)
            WaveText.GetComponent<Text>().text = "Boss Wave Starting...";
        else
            WaveText.GetComponent<Text>().text = "Wave " + wave + " Starting...";

        StartCoroutine(ClearWaveText());
    }

    IEnumerator ClearWaveText()
    {
        yield return new WaitForSeconds(2);
        WaveText.GetComponent<Text>().text = "";
    }


    IEnumerator FlickerGameOverText()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            GameOverText.SetActive(!GameOverText.activeSelf);
        }
        

    }

    IEnumerator FlickerNoAmmoText()
    {
        while (_noAmmo)
        {
            if(!_noAmmo)
                noAmmoText.SetActive(false);
            else
            {
                yield return new WaitForSeconds(0.5f);
                noAmmoText.SetActive(!noAmmoText.activeSelf);

            }
            if (!_noAmmo)
                noAmmoText.SetActive(false);

        }


    }
}
