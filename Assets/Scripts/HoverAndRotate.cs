// 4/25/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverAndRotate : MonoBehaviour
{
    public Transform cube; // Reference to the cube
    private Vector3 initialPosition; // Initial position of the cube
    public float rotationSpeed = 50f; // Rotation speed

    GameManager gameManager;
    public bool enableGrapple;
    public bool enableTonguePunch;

    // Reference to the UI text element
    public TextMeshProUGUI uiText;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = cube.position; // Initialize the position
        gameManager = FindObjectOfType<GameManager>();
        Collider collider = cube.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new position for a smooth up and down motion
        Vector3 newPosition = initialPosition + Vector3.up * Mathf.Sin(Time.time) * 0.5f;
        cube.position = newPosition; // Update the cube's position

        // Calculate the new rotation
        Quaternion rotation = Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
        cube.rotation = cube.rotation * rotation; // Update the cube's rotation
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (enableGrapple)
            {
                gameManager.enableGrapple = true;
                // Show text message on screen
                uiText.text = "Press F while the cursor is lighting up to pull yourself towards that point!";
                
            }
            else if (enableTonguePunch)
            {
                gameManager.enableTongue = true;
                uiText.text = "Press the left mouse button to punch!";
            }
            Invoke(nameof(DeletDis), 5f);
        }
    }

    private void DeletDis()
    {
        uiText.text = "";
        Destroy(gameObject);
    }
}
