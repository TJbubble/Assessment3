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

            // ����ȫ�ĵ��÷�ʽ
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
        // ��ȡ��������
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // �ƶ�����
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // ��ת����
        float rotation = turnInput * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}