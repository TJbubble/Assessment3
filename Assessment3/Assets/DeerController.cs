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
        // ��ȡ��������
        float moveInput = Input.GetAxis("Vertical"); // W/S����/�¼�ͷ
        float turnInput = Input.GetAxis("Horizontal"); // A/D����/�Ҽ�ͷ

        // �ƶ�����
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // ��ת����
        float rotation = turnInput * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // �򵥶������ƣ���ѡ��
        if (moveInput != 0)
        {
            // �����������ܲ���������
        }
    }
}