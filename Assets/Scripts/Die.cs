using UnityEngine;

/// <summary>
/// This class is responsible for handling the death of the enemy.
/// It's activated when the enemy is hit by the player's tongue.
/// </summary>
public class Die : MonoBehaviour
{
    private EnemyAI AI;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        AI = GetComponent<EnemyAI>();
        AI.enabled = false;
        audioManager.Play("Enemy_Death Sound");
        Debug.Log("damn, i fucking died");
        //TODO: implement death animation
    }
    
}
