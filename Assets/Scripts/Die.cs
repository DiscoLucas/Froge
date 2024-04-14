using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private EnemyAI AI;

    // Start is called before the first frame update
    void Start()
    {
        AI = GetComponent<EnemyAI>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tongue attack"))
        {
            AI.enabled = false;
            Debug.Log("damn, i fucking died");
            //TODO: implement death animation
        }
    }
}
