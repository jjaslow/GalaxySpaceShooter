using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private int _speed = 3;
    [SerializeField]
    private int powerupID;
    //0 triple shot
    //1 speed
    //2 shield
    //3 auto fire



    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4)
            Destroy(gameObject);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            collision.gameObject.GetComponent<Player>().PlayPowerUpClip();

            switch (powerupID)
            {
                case 0:
                    collision.gameObject.GetComponent<Player>().TripleShotPowerUp();
                    break;
                case 1:
                    collision.gameObject.GetComponent<Player>().SpeedPowerUp();
                    break;
                case 2:
                    collision.gameObject.GetComponent<Player>().ShieldPowerUp();
                    break;
                case 3:
                    collision.gameObject.GetComponent<Player>().AutoFirePowerUp();
                    break;
                case 4:
                    collision.gameObject.GetComponent<Player>().ReloadPowerUp();
                    break;
                case 5:
                    collision.gameObject.GetComponent<Player>().AddLifePowerUp();
                    break;
            }

            Destroy(gameObject);
        }
    }
}



