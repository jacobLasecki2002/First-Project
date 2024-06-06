using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player transform
    public Vector3 offset; // Offset from the player

    public float smoothSpeed = 0.125f; // Smooth speed for following
    public float mouseSensitivity = 100f; // Sensitivity of the mouse

    private float pitch = 0f; // Rotation around the X axis
    private float yaw = 0f; // Rotation around the Y axis

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        InitializeCameraPosition();
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Clamp the pitch to avoid flipping the camera

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void InitializeCameraPosition()
    {
        // Set the camera's position
        transform.position = target.position + offset;

        // Calculate the initial yaw and pitch to look at the player
        Vector3 directionToTarget = target.position - transform.position;
        Quaternion initialRotation = Quaternion.LookRotation(directionToTarget);
        yaw = initialRotation.eulerAngles.y;
        pitch = initialRotation.eulerAngles.x;

        // Set the initial camera rotation
        transform.rotation = initialRotation;
    }
}
