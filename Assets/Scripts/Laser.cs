using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8f;


    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag=="laser")
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        else if(gameObject.tag == "laserEnemy")
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        else if (gameObject.tag == "laserEnemyUp")
            transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject, 2.5f);
        Destroy(gameObject, 2.6f);
    }
}
