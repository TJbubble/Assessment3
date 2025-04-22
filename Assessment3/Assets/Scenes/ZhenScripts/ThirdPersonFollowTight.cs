using UnityEngine;
using Controller;


public class ThirdPersonFollowTight : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -4);
    public float followSpeed = 10f;
    public float mouseSensitivity = 3f;

    public float minPitch = -20f;
    public float maxPitch = 60f;

    public float autoAlignDelay = 2f;
    public float autoAlignSpeed = 2f;

    private float yaw = 0f;
    private float pitch = 15f;
    private float lastMouseInputTime = 0f;

    private MovePlayerInput playerInput;

    void Start()
    {
        if (target)
        {
            yaw = target.eulerAngles.y;
            playerInput = target.GetComponent<MovePlayerInput>();
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        bool isMouseMoving = Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f;

        // 鼠标控制视角
        if (isMouseMoving)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            lastMouseInputTime = Time.time;
        }
        // 仅在“前进”时触发自动回正
        else if (playerInput != null && playerInput.IsMovingForward && Time.time - lastMouseInputTime > autoAlignDelay)
        {
            float targetYaw = target.eulerAngles.y;
            yaw = Mathf.LerpAngle(yaw, targetYaw, Time.deltaTime * autoAlignSpeed);
        }

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Vector3 lookAtPoint = target.position + Vector3.up * 1.5f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAtPoint - transform.position), followSpeed * Time.deltaTime);
    }
}
