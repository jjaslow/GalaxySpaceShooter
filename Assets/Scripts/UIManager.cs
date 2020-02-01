using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameObject noAmmoText;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private GameObject RestartText;

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
        noAmmoText.SetActive(false);
        RestartText.SetActive(false);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
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
            
    }

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

    public void GameOver()
    {
        noAmmoText.SetActive(false);
        _noAmmo = false;
        StopCoroutine(FlickerNoAmmoText());

        GameOverText.SetActive(true);
        RestartText.SetActive(true);
        StartCoroutine(FlickerGameOverText());
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
