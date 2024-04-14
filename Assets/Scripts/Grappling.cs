using JohnStairs.RCC.Character.Motor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// inspired by https://www.youtube.com/watch?v=TYzZsBl3OI0

public class Grappling : MonoBehaviour
{
    [Header("References")]
    RPGController pm;
    public Transform cam;
    public Transform Mouth;
    public LayerMask whatIsGrappleable;

    [Header("Grapple Settings")]
    public float maxGrappleDistance;
    public float grappleDelay;
    Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCooldown;
    float grapplingCooldownTimer;

    bool isGrappling;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<RPGController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
