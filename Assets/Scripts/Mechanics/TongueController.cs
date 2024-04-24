// 4/20/2024 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using System.Collections;
using UnityEngine;

public class TongueController : MonoBehaviour
{
    public GameObject tongue; // Reference to the tongue GameObject
    public Collider tongueCollider; // Reference to the tongue collider
    public float maxDistance = 10f; // Maximum distance that the tongue can reach
    public float speed = 5f; // Speed that the tongue moves
    private Vector3 startPosition; // Original position of the tongue

    public bool isRetracting = false;
    public bool isShooting = false;

    [SerializeField] AnimationCurve extendCurve;
    [SerializeField] AnimationCurve retractCurve;
    public float time;
    public float maxTime;

    public Transform cameraOrientation;
    public Transform playerModel;//dum m�de at g�re dette p�
    public Transform startToungeTransform;
    Vector3 aimRotation;//
    public LineRenderer lineRenderer;

    private void Start()
    {
        // Save the original position of the tongue
        startPosition = tongue.transform.localPosition;
        tongueCollider = tongue.GetComponent<Collider>();
        tongueCollider.enabled = false; // Disable the collider until the tongue is extended
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isShooting)
        {
            isShooting = true;
            aimRotation = new Vector3(cameraOrientation.forward.x, cameraOrientation.forward.y, cameraOrientation.forward.z);
            playerModel.forward = aimRotation;
        }

        if (isShooting) ExtendTongue();
        

        if (isRetracting)
        {
            StartCoroutine(RetractTongue());
        }

        lineRenderer.SetPosition(0, startToungeTransform.position);
        lineRenderer.SetPosition(1, tongue.transform.position);
    }
    private void ExtendTongue()
    {
        tongueCollider.enabled = true; // Enable the collider when the tongue is extended
        time += Time.deltaTime / maxTime;
        // Move tongue forward along local axis
        float curveValue = extendCurve.Evaluate(time);
        tongue.transform.localPosition += (curveValue * speed) * Time.deltaTime * aimRotation;


        // Check if tongue has reached max distance
        if (Vector3.Distance(tongue.transform.localPosition, startPosition) >= maxDistance)
        {
            isShooting = false;
            isRetracting = true;
            time = 0;
        }
    }

    public IEnumerator RetractTongue()
    {
        time += Time.deltaTime / maxTime;
        float curveValue = retractCurve.Evaluate(time);

        // Move tongue back to original position
        tongue.transform.localPosition = Vector3.MoveTowards(tongue.transform.localPosition, startPosition, speed * curveValue * Time.deltaTime);

        // Check if tongue has returned to start position
        if (tongue.transform.localPosition == startPosition)
        {
            isRetracting = false;
            tongueCollider.enabled = false; // Disable the collider when the tongue is retracted
            StopCoroutine(RetractTongue());
        }
        yield return null;
    }

}