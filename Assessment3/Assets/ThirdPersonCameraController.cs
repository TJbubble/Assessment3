using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("基本参数")]
    public float distance = 5.0f;
    public float height = 2.0f;
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    [Header("碰撞检测")]
    public float cameraCollisionOffset = 0.2f;
    public LayerMask collisionLayers;

    [Header("鼠标控制")]
    public bool enableMouseControl = true;
    public float mouseSensitivity = 2f;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        if (enableMouseControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (enableMouseControl)
        {
            HandleMouseInput();
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleCameraPosition();
        HandleCameraRotation();
    }

    void HandleMouseInput()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -30, 60);
    }

    void HandleCameraPosition()
    {
        Vector3 targetPos = target.position + Vector3.up * height;
        Vector3 cameraDir = (transform.position - targetPos).normalized;
        float targetDistance = distance;

        RaycastHit hit;
        if (Physics.Raycast(targetPos, cameraDir, out hit, distance, collisionLayers))
        {
            targetDistance = hit.distance - cameraCollisionOffset;
        }

        transform.position = targetPos + cameraDir * targetDistance;
    }

    void HandleCameraRotation()
    {
        float wantedRotationAngle = target.eulerAngles.y + mouseX;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(
            currentRotationAngle,
            wantedRotationAngle,
            rotationDamping * Time.deltaTime);

        currentHeight = Mathf.Lerp(
            currentHeight,
            wantedHeight,
            heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(mouseY, currentRotationAngle, 0);
        transform.rotation = currentRotation;
    }
}