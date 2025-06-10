using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 3f;
    public float scrollSpeed = 5f;
    public float minPitch = 20f;
    public float maxPitch = 60f;
    public float minDistance = 4f;
    public float maxDistance = 12f;

    private float yaw = 0f;
    private float pitch = 45f;
    private float currentDistance = 8f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        Cursor.lockState = CursorLockMode.Locked; // 마우스 중앙 고정
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 마우스 이동으로 회전
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 마우스 휠로 줌 인/아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * scrollSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // 위치 계산
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = desiredPosition;
        transform.rotation = rotation;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}