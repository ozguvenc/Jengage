using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject[] CameraAnchors; // Assign the camera anchor gameobjects here.
    public float MoveSpeed = 1.0f; // Adjustable camera movement speed.

    private Camera mainCamera;
    private int currentAnchorIndex = 0;
    private bool isMoving = false;
    private Transform targetAnchor;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float journeyLength;
    private float journeyRotationAngle;
    private float startTime;

    private void Start()
    {
        if (CameraAnchors.Length < 3)
        {
            Debug.LogError("Please assign at least 3 camera anchors.");
            return;
        }

        mainCamera = Camera.main;
        if (!mainCamera)
        {
            Debug.LogError("Main camera not found.");
            return;
        }

        // Initially position the camera at the first anchor.
        mainCamera.transform.position = CameraAnchors[0].transform.position;
        mainCamera.transform.rotation = CameraAnchors[0].transform.rotation;
    }

    private void Update()
    {
        if (isMoving)
        {
            float distCovered = (Time.time - startTime) * MoveSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            // Using Lerp for position and Slerp for rotation to create the described motion.
            mainCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetAnchor.position,
                fractionOfJourney
            );
            mainCamera.transform.rotation = Quaternion.Slerp(
                startRotation,
                targetAnchor.rotation,
                fractionOfJourney
            );

            // Stop moving when close enough to the target.
            if (fractionOfJourney >= 1f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveCameraLeft()
    {
        if (isMoving)
            return;

        currentAnchorIndex--;
        if (currentAnchorIndex < 0)
            currentAnchorIndex = CameraAnchors.Length - 1;

        MoveToAnchor(CameraAnchors[currentAnchorIndex].transform);
    }

    public void MoveCameraRight()
    {
        if (isMoving)
            return;

        currentAnchorIndex++;
        if (currentAnchorIndex >= CameraAnchors.Length)
            currentAnchorIndex = 0;

        MoveToAnchor(CameraAnchors[currentAnchorIndex].transform);
    }

    private void MoveToAnchor(Transform anchor)
    {
        startTime = Time.time;
        targetAnchor = anchor;
        startPosition = mainCamera.transform.position;
        startRotation = mainCamera.transform.rotation;
        journeyLength = Vector3.Distance(mainCamera.transform.position, anchor.position);
        isMoving = true;
    }
}
