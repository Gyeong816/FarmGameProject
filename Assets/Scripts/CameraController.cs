using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -6);
    public float rotationSpeed = 3f;
    public float distance = 6f;
    public float height = 3f;

    private float yaw = 0f;
    private float pitch = 15f;

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, 5f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // 시선은 캐릭터 상체 중심
    }
}
