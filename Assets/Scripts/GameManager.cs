using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    Grappling grapplingComponent;
    TongueController tongueController;
    PlayerMovement playerMovement;
    Inputs inputs;

    [Header("Enable Mechanics")]
    public bool enableGrapple;
    public bool enableTongue;

    [Header("Player Attributes")]
    public static int playerHealth = 3;

    [Header("UI")]
    public GameObject pauseMenuUI;
    public static bool gameIsPaused = false;

    // Start is called before the first frame update
    void Awake()
    {
        // If Gamemanager is already in the scene, destroy this one
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        
        // Get references to components
        grapplingComponent = FindObjectOfType<Grappling>();
        tongueController = FindObjectOfType<TongueController>();
        
        // Get UI actionmap from "inputs"
        inputs = new Inputs();
        inputs.UI.Enable();
        inputs.UI.Pausetoggle.performed += ctx => TogglePause();

        // hide pause menu
        pauseMenuUI.SetActive(false);
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

    void TogglePause()
    {
        if (gameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Resume();
    }
    
}
