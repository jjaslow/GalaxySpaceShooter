using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodgeLaser : MonoBehaviour
{
    GameObject parentEnemy;

    // Start is called before the first frame update
    void Start()
    {
        parentEnemy = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="laser")
        {
            int leftOrRight;
            if (Random.value < 0.5f)
                leftOrRight = -3;
            else
                leftOrRight = 3;

            parentEnemy.transform.position = parentEnemy.transform.position + Vector3.right * leftOrRight;

        }
    }
}
