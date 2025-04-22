using UnityEngine;

public class ThirdPersonFollowTight1 : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float followSpeed = 8f;
    public float rotSpeed    = 4f;
    public float alignDelay  = 2f;
    public float alignSpeed  = 2f;
    public float minPitch    = -20f;
    public float maxPitch    = 60f;

    float yaw, pitch;
    float lastMouseTime;

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y; pitch = e.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        bool mouseMoved = Mathf.Abs(mx) > 0.01f || Mathf.Abs(my) > 0.01f;

        if (mouseMoved)
        {
            yaw += mx * rotSpeed;
            pitch -= my * rotSpeed;
            lastMouseTime = Time.time;
        }
        else if (Time.time - lastMouseTime > alignDelay)
        {
            float ty = target.eulerAngles.y;
            yaw = Mathf.LerpAngle(yaw, ty, Time.deltaTime * alignSpeed);
        }

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPos = target.position + rot * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
        Vector3 lookPoint = target.position + Vector3.up * 1.5f;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookPoint - transform.position),
            followSpeed * Time.deltaTime
        );
    }
}
