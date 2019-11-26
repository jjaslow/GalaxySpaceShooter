using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private GameObject RestartText;

    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _livesImagePrefab;


    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: 0";
        GameOverText.SetActive(false);
        RestartText.SetActive(false);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateLivesImage(int lives)
    {
        if(lives >=0)
        {
            _livesImage.sprite = _livesImagePrefab[lives];
        }
        
    }

    public void GameOver()
    {
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
}
