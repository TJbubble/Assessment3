using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class CreatureMover1 : MonoBehaviour
    {
        [Header("Speeds")]
        public float walkSpeed = 2f;        // 行走速度
        public float sprintSpeed = 6f;      // 奔跑速度
        public float turnSpeed = 8f;        // 旋转速度

        [Header("Jump Settings")]
        public float jumpHeight = 2f;       // 跳跃高度
        public float gravity = -9.81f;      // 重力

        [Header("Animator Parameters")]
        public string speedID = "Speed";    // 控制行走和奔跑的参数
        public string sprintID = "IsSprinting"; // 控制是否冲刺的参数
        public string jumpTriggerID = "JumpTrigger";  // 跳跃动画触发
        public string fallBoolID = "FallBool";   // 下落状态

        private CharacterController cc;
        private Animator animator;

        private Vector2 inputAxis;
        private Vector3 moveDir;
        private Vector3 velocity;
        private bool isJumping;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        public void SetInput(Vector2 axis, Vector3 dir, bool sprint, bool jump)
        {
            inputAxis = axis;
            moveDir = dir * (sprint ? sprintSpeed : walkSpeed);  // 计算行走或奔跑方向
            isJumping = jump;

            // 更新冲刺状态
            animator.SetBool(sprintID, sprint);
        }

        void Update()
        {
            // 处理重力和跳跃
            if (cc.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;  // 角色接触地面时不再继续掉落
                animator.SetBool(fallBoolID, false);  // 不在空中
            }

            // 如果按了跳跃键并且在地面上
            if (isJumping && cc.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger(jumpTriggerID);  // 触发跳跃动画
            }

            // 应用重力
            if (!cc.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                animator.SetBool(fallBoolID, true);  // 在空中时播放掉落动画
            }

            // 角色旋转：根据输入方向进行旋转
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * turnSpeed  // 平滑转向
                );
            }

            // 移动
            Vector3 move = moveDir * Time.deltaTime;
            cc.Move(move + velocity * Time.deltaTime);

            // **更新 Speed 参数**：根据实际移动速度计算
            // 注意：如果角色停下来，speed 应该为 0（Idle），否则根据速度值来过渡
            float speed = move.magnitude / (sprintSpeed * Time.deltaTime);
            animator.SetFloat(speedID, speed, 0.1f, Time.deltaTime);  // 动画控制的关键参数
        }
    }
}
