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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cherry"))
        {
            Destroy(other.gameObject);

            // 更安全的调用方式
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.CollectCherry();
            }
            else
            {
                Debug.LogError("GameManager not found in scene!");
            }
        }
    }

    void Update()
    {
        // 获取键盘输入
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // 移动控制
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // 旋转控制
        float rotation = turnInput * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}