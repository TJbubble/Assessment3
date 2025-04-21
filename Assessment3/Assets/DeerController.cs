using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 获取键盘输入
        float moveInput = Input.GetAxis("Vertical"); // W/S或上/下箭头
        float turnInput = Input.GetAxis("Horizontal"); // A/D或左/右箭头

        // 移动控制
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // 旋转控制
        float rotation = turnInput * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // 简单动画控制（可选）
        if (moveInput != 0)
        {
            // 这里可以添加跑步动画触发
        }
    }
}