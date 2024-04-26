using UnityEngine;

/// <summary>
/// This class is responsible for handling the death of the enemy.
/// It's activated when the enemy is hit by the player's tongue.
/// </summary>
public class Die : MonoBehaviour
{
    private EnemyAI AI;

    // Start is called before the first frame update
    void Start()
    {
        AI = GetComponent<EnemyAI>();
        AI.enabled = false;
        AudioManager.instance.Play("Enemy_Death Sound");
        Debug.Log("damn, i fucking died");
        //TODO: implement death animation
    }
    
}
