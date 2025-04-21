using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class CreatureMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_WalkSpeed = 1f;
        [SerializeField] private float m_RunSpeed = 4f;
        [SerializeField, Range(0f, 360f)] private float m_RotateSpeed = 90f;
        [SerializeField] private Space m_Space = Space.Self;
        [SerializeField] private float m_JumpHeight = 5f;

        [Header("Collision")]
        [SerializeField] private float m_SkinWidth = 0.08f;
        [SerializeField] private float m_MinMoveDistance = 0.001f;
        [SerializeField] private float m_ObstacleCheckDistance = 0.5f;

        [Header("Animator")]
        [SerializeField] private string m_VerticalID = "Vert";
        [SerializeField] private string m_StateID = "State";
        [SerializeField] private LookWeight m_LookWeight = new(1f, 0.3f, 0.7f, 1f);

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;
        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;
        private bool m_IsMoving;

        private void OnValidate()
        {
            m_WalkSpeed = Mathf.Max(m_WalkSpeed, 0f);
            m_RunSpeed = Mathf.Max(m_RunSpeed, m_WalkSpeed);
            m_SkinWidth = Mathf.Clamp(m_SkinWidth, 0.01f, 0.2f);
            m_MinMoveDistance = Mathf.Max(m_MinMoveDistance, 0f);
        }

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            m_Controller.skinWidth = m_SkinWidth;
            m_Controller.minMoveDistance = m_MinMoveDistance;

            m_Movement = new MovementHandler(
                m_Controller, 
                m_Transform, 
                m_WalkSpeed, 
                m_RunSpeed,
                m_RotateSpeed, 
                m_JumpHeight, 
                m_Space, 
                m_ObstacleCheckDistance
            );
            
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        private void Update()
        {
            m_Movement.Move(Time.deltaTime, m_Axis, m_IsRun, m_IsMoving, out var animAxis, out var isAir);
            m_Animation.Animate(animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(m_Target, m_LookWeight);
        }

        public void SetInput(Vector2 axis, Vector3 target, bool isRun, bool isJump)
        {
            m_Axis = axis;
            m_Target = target;
            m_IsRun = isRun;

            m_IsMoving = m_Axis.sqrMagnitude > Mathf.Epsilon;
            if (m_IsMoving)
            {
                m_Axis = Vector2.ClampMagnitude(m_Axis, 1f);
            }
            else
            {
                m_Axis = Vector2.zero;
            }
        }

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;
            private readonly float m_WalkSpeed;
            private readonly float m_RunSpeed;
            private readonly float m_RotateSpeed;
            private readonly Space m_Space;
            private readonly float m_ObstacleCheckDistance;

            private Vector3 m_Normal = Vector3.up;
            private Vector3 m_Gravity = Physics.gravity;
            private Vector3 m_LastMoveDirection;

            public MovementHandler(
                CharacterController controller,
                Transform transform,
                float walkSpeed,
                float runSpeed,
                float rotateSpeed,
                float jumpHeight,
                Space space,
                float obstacleCheckDistance)
            {
                m_Controller = controller;
                m_Transform = transform;
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;
                m_Space = space;
                m_ObstacleCheckDistance = obstacleCheckDistance;
            }

            public void Move(
                float deltaTime,
                Vector2 axis,
                bool isRun,
                bool isMoving,
                out Vector2 animAxis,
                out bool isAir)
            {
                // 计算移动方向
                CalculateMovement(axis, out var movement);
                
                // 处理旋转
                if (isMoving)
                {
                    SmoothRotation(movement, deltaTime);
                }

                // 处理重力
                CalculateGravity(deltaTime, out isAir);
                
                // 处理位移
                SafeMove(movement, isRun, deltaTime, isAir);
                
                // 生成动画参数
                CalculateAnimAxis(movement, out animAxis);
            }

            private void CalculateMovement(Vector2 axis, out Vector3 movement)
            {
                if (m_Space == Space.Self)
                {
                    // 本地空间移动
                    movement = m_Transform.forward * axis.y + m_Transform.right * axis.x;
                }
                else
                {
                    // 世界空间移动
                    movement = new Vector3(axis.x, 0, axis.y);
                }

                movement = Vector3.ProjectOnPlane(movement, m_Normal).normalized;
                
                if (movement.sqrMagnitude > 0.1f)
                {
                    m_LastMoveDirection = movement;
                }
            }

            private void SmoothRotation(Vector3 direction, float deltaTime)
            {
                if (direction.sqrMagnitude < 0.01f) return;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                m_Transform.rotation = Quaternion.Slerp(
                    m_Transform.rotation,
                    targetRotation,
                    m_RotateSpeed * deltaTime
                );
            }

            private void SafeMove(Vector3 movement, bool isRun, float deltaTime, bool isAir)
            {
                if (movement.sqrMagnitude < 0.01f)
                {
                    m_Controller.Move(m_Gravity * deltaTime);
                    return;
                }

                float speed = isRun ? m_RunSpeed : m_WalkSpeed;
                Vector3 displacement = movement * speed * deltaTime;

                // 障碍物检测
                if (CheckObstacle(movement, displacement.magnitude))
                {
                    displacement *= 0.3f;
                }

                // 应用重力
                if (!isAir || displacement.y < 0)
                {
                    displacement.y = m_Gravity.y * deltaTime;
                }

                m_Controller.Move(displacement);
            }

            private bool CheckObstacle(Vector3 direction, float distance)
            {
                float radius = m_Controller.radius;
                float height = m_Controller.height;
                Vector3 center = m_Transform.position + m_Controller.center;
                
                Vector3 bottom = center - Vector3.up * (height * 0.5f - radius);
                Vector3 top = center + Vector3.up * (height * 0.5f - radius);
                
                float checkDistance = Mathf.Min(distance + m_Controller.skinWidth, m_ObstacleCheckDistance);
                
                return Physics.CapsuleCast(bottom, top, radius, direction, checkDistance);
            }

            private void CalculateGravity(float deltaTime, out bool isAir)
            {
                isAir = !m_Controller.isGrounded;
                m_Gravity = isAir ? m_Gravity + Physics.gravity * deltaTime : Physics.gravity;
            }

            private void CalculateAnimAxis(Vector3 movement, out Vector2 animAxis)
            {
                if (m_Space == Space.Self)
                {
                    animAxis = new Vector2(
                        Vector3.Dot(movement, m_Transform.right),
                        Vector3.Dot(movement, m_Transform.forward)
                    );
                }
                else
                {
                    animAxis = new Vector2(movement.x, movement.z);
                }
            }
        }

        private class AnimationHandler
        {
            private readonly Animator m_Animator;
            private readonly string m_VerticalID;
            private readonly string m_StateID;
            private readonly float m_InputSmoothing = 4.5f;
            
            private Vector2 m_SmoothedAxis;
            private float m_SmoothedState;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            public void Animate(Vector2 axis, float state, float deltaTime)
            {
                // 平滑输入
                m_SmoothedAxis = Vector2.MoveTowards(
                    m_SmoothedAxis, 
                    Vector2.ClampMagnitude(axis, 1f), 
                    m_InputSmoothing * deltaTime
                );
                
                m_SmoothedState = Mathf.MoveTowards(
                    m_SmoothedState, 
                    Mathf.Clamp01(state), 
                    m_InputSmoothing * deltaTime
                );

                // 设置动画参数
                m_Animator.SetFloat(m_VerticalID, m_SmoothedAxis.magnitude);
                m_Animator.SetFloat(m_StateID, m_SmoothedState);
            }

            public void AnimateIK(Vector3 target, LookWeight weights)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(
                    weights.weight, 
                    weights.body, 
                    weights.head, 
                    weights.eyes
                );
            }
        }
    }
}