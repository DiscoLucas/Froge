using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueTrigger : MonoBehaviour
{
    [SerializeField] private Collider tongueCollider;
    TongueController tongueController;
    [SerializeField] private Rigidbody parentRB;
    private void Start()
    {
        tongueCollider = GetComponent<Collider>();
        tongueController = GetComponentInParent<TongueController>();
        parentRB = GetComponentInParent<Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider tongueCollider)
    {
        // get the name of the object that the tongue hit
        
        if (!tongueCollider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Tongue hit: " + tongueCollider.gameObject.name);

            if (tongueCollider.gameObject.CompareTag("Enemy"))
            {
                tongueCollider.gameObject.GetComponent<Die>().enabled = true;
            }
            tongueController.isShooting = false;
            tongueController.isRetracting = true; // hacky solution, but it works
            //while (tongueController.isRetracting) StartCoroutine(tongueController.RetractTongue());

        }
    }
    /*private void OnCollisionEnter(Collision tongueCollider)
    {
        // get the name of the object that the tongue hit

        if (!tongueCollider.gameObject.CompareTag("Player"))
        {
            Debug.Log("Tongue hit: " + tongueCollider.gameObject.name);
            tongueController.isShooting = false;
            tongueController.isRetracting = true; // hacky solution, but it works
            //while (tongueController.isRetracting) StartCoroutine(tongueController.RetractTongue());

        }
    }*/
}
