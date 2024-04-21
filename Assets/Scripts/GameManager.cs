using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    Grappling grapplingComponent;
    TongueController tongueController;
    PlayerMovement playerMovement;

    [Header("Enable Mechanics")]
    public bool enableGrapple;
    public bool enableTongue;

    [Header("Player Attributes")]
    public int playerHealth = 3;

    // Start is called before the first frame update
    void Start()
    {
        // If Gamemanager is already in the scene, destroy this one
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        grapplingComponent = GetComponent<Grappling>();
        tongueController = GetComponent<TongueController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enableGrapple) grapplingComponent.enabled = true;
        else grapplingComponent.enabled = false;

        if (enableTongue) tongueController.enabled = true;
        else tongueController.enabled = false;

        if (playerHealth <= 0)
        {
            // Game Over
            Debug.Log("Game Over");
        }
    }
    
}
