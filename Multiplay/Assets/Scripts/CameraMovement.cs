using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot; // Assign MainCamera here
    [SerializeField] private float scrollSensitivity = 5f;
    [SerializeField] private float lerpSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minZoom = -20f;
    [SerializeField] private float maxZoom = -5f;

    private float targetZoom;
    private Vector2 currentRotation;
    private Camera cam;

    private void Start()
    {
        currentRotation = transform.eulerAngles;
        cam = Camera.main;
        targetZoom = cam.fieldOfView;
    }

    private void Update()
    {
        HandleZoom();
        HandleRotation();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoom -= scroll * scrollSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom); // flip if needed
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * lerpSpeed);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse drag
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentRotation.x += vertical;
            currentRotation.y += horizontal;

            // Optionally clamp vertical rotation
            currentRotation.x = Mathf.Clamp(currentRotation.x, -80, 80);

            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        }
    }
}
