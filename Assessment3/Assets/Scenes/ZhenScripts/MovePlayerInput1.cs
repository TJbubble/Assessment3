using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover1))]
    public class MovePlayerInput1 : MonoBehaviour
    {
        [Header("Character Settings")]
        public string horizontal = "Horizontal";
        public string vertical   = "Vertical";
        public string jumpButton = "Jump";
        public KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Camera")]
        public Transform cam;

        private CreatureMover1 mover;
        private Vector2 axis;
        private bool isJumping, isSprinting;

        void Awake()
        {
            mover = GetComponent<CreatureMover1>();
        }

        void Update()
        {
            float h = Input.GetAxis(horizontal);
            float v = Input.GetAxis(vertical);
            axis = new Vector2(h, v);

            isSprinting = Input.GetKey(sprintKey); // 检查是否按下冲刺键
            isJumping = Input.GetButtonDown(jumpButton); // 检查是否按下跳跃键

            // 根据摄像机旋转，计算角色的移动方向
            Vector3 forward = cam.forward; forward.y = 0; forward.Normalize();
            Vector3 right   = cam.right;   right.y   = 0; right.Normalize();
            Vector3 direction = (right * h + forward * v).normalized;

            mover.SetInput(axis, direction, isSprinting, isJumping);
        }
    }
}
