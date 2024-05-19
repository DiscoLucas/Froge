using System;
using UnityEngine;

/// <summary>
/// This class is responsible for handling the death of the enemy.
/// It's activated when the enemy is hit by the player's tongue.
/// </summary>
public class Die : MonoBehaviour
{
    private EnemyAI AI;
    AudioManager audioManager;
    Transform body;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        AI = GetComponent<EnemyAI>();
        body = gameObject.transform.Find("Salamander");

        if (body != null) body.gameObject.SetActive(false); // Disable the body of the dead enemy
        else throw new Exception ("Salamander body not found");

        AI.enabled = false;
        audioManager.Play("Enemy_Death Sound");

        

        // Play child particle system once
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }

    }
    

    public void Despawn()
    {
        Destroy(gameObject);
    }
    
}
