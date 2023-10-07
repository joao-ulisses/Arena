using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Inimigo"))
        {
            other.gameObject.GetComponent<EnemyAiWaypoint>().TakeDamage(4);
            Destroy(this.gameObject);
        }
    }
}